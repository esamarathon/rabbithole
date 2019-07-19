namespace rabbithole {

    public class RabbitConfig {
        public string HostName { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string VirtualHost { get; set; }

        public SslOptions Ssl { get; set; }

        public Binding[] Bindings { get; set; }        
    }

    public class SslOptions {
        public bool Enabled { get; set; }

        public string ServerName { get; set; }
    }

    public class Binding {
        public string Exchange { get; set; }

        public string Topic { get; set; }
    }
}