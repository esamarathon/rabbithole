namespace rabbithole {

    public class RabbitConfig {
        public string HostName { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string VirtualHost { get; set; }

        public Binding[] Bindings { get; set; }        
    }

    public class Binding {
        public string Exchange { get; set; }

        public string Topic { get; set; }
    }
}