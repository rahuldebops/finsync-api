namespace finsyncapi.BAL.Services
{
    public interface ISnowflakeService
    {
        long NextId();
        DateTimeOffset ExtractTimestamp(long id);
    }

    public class SnowflakeService : ISnowflakeService
    {
        private const long Twepoch = 1288834974657L;
        private const int WorkerIdBits = 10;
        private const int SequenceBits = 12;

        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);

        private readonly object _lock = new();
        private readonly long _workerId;
        private long _lastTimestamp = -1L;
        private long _sequence = 0L;

        private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string Base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public SnowflakeService(IConfiguration configuration)
        {
            // Load from config for now, future: inject dynamic resolver
            var workerId = configuration.GetValue<long>("Snowflake:WorkerId", 0);

            if (workerId < 0 || workerId > MaxWorkerId)
                throw new ArgumentException($"Worker ID must be between 0 and {MaxWorkerId}");

            _workerId = workerId;
        }

        public long NextId()
        {
            lock (_lock)
            {
                var timestamp = GetCurrentTimestamp();

                if (timestamp < _lastTimestamp)
                    throw new InvalidOperationException("Clock moved backwards");

                if (timestamp == _lastTimestamp)
                {
                    _sequence = (_sequence + 1) & SequenceMask;
                    if (_sequence == 0)
                        timestamp = WaitForNextMillis(_lastTimestamp);
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;

                return ((timestamp - Twepoch) << (WorkerIdBits + SequenceBits)) |
                       (_workerId << SequenceBits) |
                       _sequence;
            }
        }

        public DateTimeOffset ExtractTimestamp(long id)
        {
            var timestamp = (id >> (WorkerIdBits + SequenceBits)) + Twepoch;
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
        }

        private long GetCurrentTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        private long WaitForNextMillis(long lastTimestamp)
        {
            var timestamp = GetCurrentTimestamp();
            while (timestamp <= lastTimestamp)
                timestamp = GetCurrentTimestamp();
            return timestamp;
        }
    }
}
