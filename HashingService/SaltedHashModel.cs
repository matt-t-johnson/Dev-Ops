namespace HashingService
{
    public class SaltedHashModel
    {
        public string Salt { get; set; }
        public string HashedValue { get; set; }
        public string InputValue { get; set; }
    }
}