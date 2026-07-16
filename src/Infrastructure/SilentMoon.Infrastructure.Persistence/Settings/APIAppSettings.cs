
namespace SilentMoon.Infrastructure.Persistence.Settings
{
    public class APIAppSettings
    {
        public string ConnectionString { get; set; }
        public string ClientAppOrigin { get; set; }
        public MailSettings MailSettings { get; set; }
        public JWTSettings JWTSettings { get; set; }
        public GoogleAuthSettings GoogleAuth { get; set; }     
        public FacebookAuthSettings FacebookAuth { get; set; }
        public RabbitMqSettings RabbitMq { get; set; }
    }
}