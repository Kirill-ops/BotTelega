namespace BotTelega;

internal class Program
{
    static async Task Main(string[] args)
    {

        ClientBotTelegram bot = new ClientBotTelegram();
        Console.WriteLine("Сервер запущен...");
        await bot.GetAsync();
        while (true)
        {
            await bot.GetAsync();
        }

    }

}





/*
        object? test = null;

        if (test != null)
        {
            object notNull = test;
        }

        int? testInt = null;

        if (testInt != null)
        {
            int notNullInt = testInt.Value;
        }
*/

/*
         var t = telegramResponse.Result[0].Message?.Chat?.Id;

         if (t != null)
         {
         }
*/


/*ApplicationContext db = new ApplicationContext();

        db.ReservedIPv6.Add(new ModelReservedIPv6 { 
            StartAddres = "::", 
            Prefix = 128, 
            Description = "None", 
            Note = "Нулевой адрес, используется для указания неизвестного адреса или для установления соединения с самим собой" });
        db.ReservedIPv6.Add(new ModelReservedIPv6
        {
           StartAddres = "::1",
           Prefix = 128,
           Description = "loopback адрес",
           Note = "Адрес для циклической обратной связи (localhost)."
        });

        db.ReservedIPv6.Add(new ModelReservedIPv6
        {
            StartAddres = "::xx.xx.xx.xx",
            Prefix = 96,
            Description= "встроенный IPv4",
            Note = "Нижние 32 бита это адрес IPv4. Также называется IPv4-совместимым IPv6 адресом. Устарел и больше не используется."
        });

        db.ReservedIPv6.Add(new ModelReservedIPv6
        {
            StartAddres = "::ffff:​xx.xx.xx.xx",
            Prefix = 96,
            Description = "Адрес IPv4, отображённый на IPv6",
            Note = "Нижние 32 бита — это адрес IPv4 для хостов, не поддерживающих IPv6."

        });

        db.ReservedIPv6.Add(new ModelReservedIPv6
        {
            StartAddres = "64:ff9b::",
            Prefix = 96,
            Description = "NAT64",
            Note = "Зарезервирован для доступа из подсети IPv6 к публичной сети IPv4 через механизм трансляции NAT64."
        });

        db.ReservedIPv6.Add(new ModelReservedIPv6
        {
            StartAddres = "2001:2::",
            Prefix = 48,
            Description = "Benchmarks",
            Note = "Используется для проведения тестов производительности сети."
        });

        db.ReservedIPv6.Add(new ModelReservedIPv6
        {
            StartAddres = "2001::",
            Prefix = 32,
            Description = "Teredo",
            Note = "Зарезервирован для туннелей Teredo в RFC 4380."
        });

        db.ReservedIPv6.Add(new ModelReservedIPv6
        {
            StartAddres = "2001:db8::",
            Prefix = 32,
            Description = "Документирование",
            Note = "Зарезервирован для примеров в документации в RFC 3849."
        });

        db.ReservedIPv6.Add(new ModelReservedIPv6
        {
            StartAddres = "2002::",
            Prefix = 16,
            Description = "6to4",
            Note = "Зарезервирован для туннелей 6to4 в RFC 3056."
        });

        db.ReservedIPv6.Add(new ModelReservedIPv6
        {
            StartAddres = "fe80::",
            EndAddres = "febf::",
            Prefix = 10,
            Description = "link-local",
            Note = "Локальные ссылочные адреса, используются для связи внутри локальной сети. Аналог 169.254.0.0/16 в IPv4."
        });

        db.ReservedIPv6.Add(new ModelReservedIPv6
        {
            StartAddres = "fec0::",
            EndAddres = "feff::",
            Prefix = 10,
            Description = "site-local",
            Note = "Помечен как устаревший в RFC 3879. Были предназначены для использования в ограниченном масштабе, в пределах одного сайта (site), то есть в рамках одной организации или локальной сети."
        });

        db.ReservedIPv6.Add(new ModelReservedIPv6
        {
            StartAddres = "fc00::",
            Prefix = 7,
            Description = "Unique Local Unicast",
            Note = "Пришёл на смену Site-Local RFC 4193. Предназначены для использования в локальных сетях, но они обеспечивают глобальную уникальность и могут использоваться без опасения конфликтов адресов."
        });

        db.ReservedIPv6.Add(new ModelReservedIPv6
        {
            StartAddres = "ff00::",
            Prefix = 8,
            Description = "Multicast",
            Note = "Адреса мультикаста используются для отправки пакетов на группу узлов, которые имеют общий интерес к этим данным. RFC 3513"
        });

        db.SaveChanges();*/