namespace AbpFramework.OTel
{
    public class OTelConsts
    {
        public const string LocalizationSourceName = "OTel";

        public const bool MultiTenancyEnabled = true;
        
        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public const string DefaultPassPhrase = "{{DEFAULT_PASS_PHRASE_HERE}}";
    }
}