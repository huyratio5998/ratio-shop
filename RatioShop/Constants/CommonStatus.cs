namespace RatioShop.Constants
{
    public static class CommonStatus
    {
        public const string Success = "Success";
        public const string Failure = "Failure";
        public static class CartStatus
        {
            public const string Created = "Created";
            public const string InOrderProcess = "InOrderProcess";
            public const string Complete = "Complete";
            public const string AbandonedCart = "AbandonedCart";
            public const string Cancel = "Cancel";
        }
        public static class OrderStatus
        {
            public const string Created = "Created";
            public const string PendingPayment = "Pending Payment";
            public const string PaymentRecieved = "Payment Recieved";
            public const string Delivering = "Delivering";
            public const string Canceled = "Canceled";

            public const string Complete = "Complete";
            public const string Closed = "Closed";
        }
        public static class ShipmentStatus
        {
            public const string Pending = "Pending";
            public const string Delivering = "Delivering";
            public const string Failure = "Failure"; // fail when returning ? fail when delivering
            public const string Canceled = "Canceled"; // => could be Returning -> Returned  ---- or Closed
            public const string Returning = "Returning";

            public const string Closed = "Closed";
            public const string Returned = "Returned";
            public const string Expired = "Expired";
            public const string Delivered = "Delivered"; // => could be Returning -> Returned ------ or Delivered
        }
        public static class Discount
        {
            public const string Active = "Active";
            public const string InActive = "InActive";           
        }        
        public static class Tracking
        {
            public const string Active = "Active";
            public const string InActive = "InActive";
        }
    }
}
