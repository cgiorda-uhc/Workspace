namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Abstract
{
    public interface IEmailConfig
    {
        public string EmailTo { get; set; }

        public string EmailFrom { get; set; }

        public string EmailCC { get; set; }

        public string EmailSubject { get; set; }

        public string EmailBody { get; set; }

        public Status EmailStatus { get; set; }

    }
}
