namespace finsyncapi.Helper
{
    public static class AppConstant
    {
        public static int ACCESSTOKEN_EXPIRES_SEC = 300;
        public static int REFRESHTOKEN_EXPIRES_SEC = 86400;

        public static int OTP_LENGTH = 6;
        public static int OTP_MAX_ATTEMPT = 3;
        public static int OTP_EXPIRES_SEC = 300;

        public static int OTP_RESEND_COOLDOWN_SEC = 1;
        public static int OTP_MAX_RESEND_COUNT = 30;
        public static int OTP_MAX_RESEND_WINDOW_HOURS = 2;
    }
    public static class QueueNames
    {
        public static readonly string SendOtpEmail = "send-otp-email";
    }
    public static class Messages
    {
        // General Errors
        public static readonly string SomethingWentWrong = "Something went wrong. Please try again.";
        public static readonly string EmailOrPhoneAlreadyRegistered = "Email or phone number is already registered.";
        public static readonly string InvalidCredentials = "Invalid credentials. Please check your login details.";
        public static readonly string InvalidRefreshToken = "Invalid refresh token.";
        public static readonly string InvalidAccessToken = "Invalid access token.";
        public static readonly string JWTKeyIsMissing = "JWT key is missing.";
        public static readonly string UnAuthorized = "You are unauthorized to perform this action.";

        // Auth
        public static readonly string LoginSuccessfully = "Logged in successfully.";
        public static readonly string LogoutSuccessfully = "Access token refreshed successfully.";
        public static readonly string UserAlreadyVerified = "User already verified.";
        public static readonly string UserAlreadyRegistered = "User already registered.";
        public static readonly string EmailIsRequired = "Email is required.";
        public static readonly string PhoneNumberIsRequired = "Phone number is required.";
        public static readonly string UserNotFound = "User not found.";
        public static readonly string UserEmailNotFound = "User's email not found.";
        public static readonly string UserPhoneNumberNotFound = "User's phone number not found.";
        public static readonly string InvalidRegistrationProvider = "Invalid registration provider.";

        // Friend Requests
        public static readonly string FriendRequestSent = "Friend request sent successfully.";
        public static readonly string FriendRequestAccepted = "Friend request accepted.";
        public static readonly string FriendRequestRejected = "Friend request rejected.";
        public static readonly string AlreadyInFriend = "You are already in friend";
        public static readonly string FriendRequestAlreadySent = "You have already sent the request";
        public static readonly string UserHaveAlreadySentRequestToYou = "User have already sent request to you. Check pending list to confirm";

        // Transction
        public static readonly string TransactionAdded = "New transaction added successfully.";
        public static readonly string TransactionUpdated = "Transaction updated successfully.";
        public static readonly string TransactionFetched = "Transaction fetched successfully.";
        public static readonly string InvalidTransactionType = "Invalid or inactive transaction type.";
        public static readonly string GroupNotAllowedForType = "This transaction type is not allowed for group transactions.";
        public static readonly string InvalidCategory = "Invalid or unauthorized category for this transaction.";
        public static readonly string InvalidAccount = "One or more accounts are invalid or not owned by you.";
        public static readonly string GroupNotFound = "Group not found or inactive.";
        public static readonly string NotInGroup = "You are not a member of this group.";
        public static readonly string SplitTotalMismatch = "The sum of split amounts does not match the total transaction amount.";
        public static readonly string InvalidSplitMember = "One or more members in the split are not in the group.";
        public static readonly string TransactionNotFound = "Transaction not found.";

        // OTP
        public static readonly string OtpSentSuccessfully = "OTP sent successfully";
        public static readonly string OtpVerifiedSuccessfully = "OTP verified successfully";
        public static readonly string InvalidOtp = "Invalid OTP";
        public static readonly string OtpExpired = "OTP expired";
        public static readonly string MaximumAttemptsReached = "Maximum attempts reached";
        public static readonly string OtpNotFound = "OTP not found";
        public static readonly string OtpLimitExceed = "OTP limit exceeded. Please wait before requesting again.";

        // NOTIFICATION
        public static readonly string EmailSendingFailed = "Email sending failed.";
        public static readonly string PrimaryEmailProviderNotFound = "Primary email provider not found";
        public static readonly string AllEmailProvidersFailed = "All email providers failed";

    }

    public static class ErrorCodes
    {
        public static string UNAUTHORIZED = "UNAUTHORIZED";
    }

    public static class ProviderConstants
    {
        public static class Selection
        {
            public static readonly string Primary = "PRIMARY";
            public static readonly string Failover = "FAILOVER";
            public static readonly string Random = "RANDOM";
        }

        public static class EmailProviders
        {
            public static readonly string Resend = "Resend";
            public static readonly string Brevo = "Brevo";
        }

        public static class Templates
        {
            public static readonly string OtpVerification = "OtpVerification";
        }

        public static class Operators
        {
            public static readonly string EqualsTo = "equals";
            public static readonly string NotEqualsTo = "notEquals";
            public static readonly string EndsWith = "endsWith";
            public static readonly string Contains = "contains";
            public static readonly string NotContains = "notContains";
            public static readonly string GreaterThan = "gt";
            public static readonly string GreaterThanEqualsTo = "gte";            
            public static readonly string LessThan = "lt";
            public static readonly string LessThanEqualsTo = "lte";

        }

        public static class MatchMode
        {
            public static readonly string And = "and";
            public static readonly string Or = "or";

        }
    }
}
