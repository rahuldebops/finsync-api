using System.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace finsyncapi.Helpers
{
    public static class Enums
    {
        public enum ClaimNames
        {
            [Description("UserId")]
            UserId,

            [Description("Phone")]
            Phone,

            [Description("Email")]
            Email,

            [Description("Role")]
            Role,

            [Description("TimeZone")]
            TimeZone
        }
        public enum UserRoleEnum
        {
            SUPERADMIN = 1,
            ADMIN = 2,
            ENDUSER = 3
        }

        public enum OrderByEnum
        {
            ASC, // ascending
            DESC // decending 
        }

        public enum OtpPurpose {
            EMAIL_VERIFICATION = 1,
            PHONE_NUMBER_VERIFICATION = 2,
            LOGIN = 3,
            PASSWORD_RESET = 4,

        }

        public enum RegistrationProvider
        {
            Email = 1,
            Phone = 2,
            Google = 3
        }

        public enum GroupRoleType
        {
            ADMIN = 1,
            MEMBER = 2
        }

        public enum ProfileRequirement
        {
            None,
            Required
        }

        public enum AppEnvironment
        {
            PROD,
            UAT,
            DEV
        }

        public enum TransactionTypeEnum : short
        {
            Expense = 1,
            Income = 2,
            Investment = 3,
            Debt = 4,
            Savings = 5,
            Cashback = 6,
            Refund = 7
        }

        public enum DebitCreditType : short
        {
            Debit = -1,
            Transfer = 0,
            Credit = 1
        }

        public enum ExpenseSplitEnum : short
        {
            EQUAL = 0,
            PERCENTAGE = 1,
            SHARE = 2,


        }

        public enum LedgerType : short
        {
            TransactionCreated = 1,
            PaymentRecorded = 2,
            ExpenseSplit = 3
        }

        public enum OperatorEnum{
             eq,
             contains,
             gt,
             gte,
             lt,
             lte
        }
    }
}
