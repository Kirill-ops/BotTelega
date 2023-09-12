using System.Text.RegularExpressions;

namespace BotTelega;

// private _firstSecond
// const FirstSecond
// public FirstSecond
// public FirstSecond

public class ResponseMessage
{
    private List<(Func<string> f, Regex reg)> _list = new();
    private List<(Func<string, string> f, Regex reg)> _listCalculator = new();

    private const int MaxPrefix = 128;
    private const int MinPrefix = 0;
    private const string MessageSeparator = "||";

    private int _maxLengthMessage = 4000;

    private IPv6Network _ipv6 = new();

    public ResponseMessage()
    {
        _list.Add((Start, new Regex(@"(/start)", RegexOptions.IgnoreCase)));
        _list.Add((Help, new Regex(@"(/help)", RegexOptions.IgnoreCase)));
        _list.Add((DescSplitSubnets, new Regex(@"(как делить\?)", RegexOptions.IgnoreCase)));
        _list.Add((DescAddSubnets, new Regex(@"(как соединять\?)", RegexOptions.IgnoreCase)));
        _list.Add((DescSplitSubnetsByCountInterfaces, new Regex(@"(как делить по кол-ву интерфейсов?)", RegexOptions.IgnoreCase)));
        _list.Add((DescriptionSubnet, new Regex(@"(основная информация о сети)", RegexOptions.IgnoreCase)));

        _listCalculator.Add((SplitSubnets, new Regex("^(\\s*)(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|" +
                        "([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|" +
                        "([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|" +
                        "([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|" +
                        "([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|" +
                        "([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|" +
                        "[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|" +
                        ":((:[0-9a-fA-F]{1,4}){1,7}|:)|" +
                        "fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|" +
                        "::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|" +
                        "1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|" +
                        "1{0,1}[0-9]){0,1}[0-9])|" +
                        "([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|" +
                        "1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|" +
                        "(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))\\s([0-9]{1,3})\\s([0-9]{1,3})(\\s*)$", RegexOptions.IgnoreCase)));
        _listCalculator.Add((DescriptionSubnet, new Regex("^((([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|" +
                        "([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|" +
                        "([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|" +
                        "([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|" +
                        "([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|" +
                        "([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|" +
                        "[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|" +
                        ":((:[0-9a-fA-F]{1,4}){1,7}|:)|" +
                        "fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|" +
                        "::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|" +
                        "1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|" +
                        "1{0,1}[0-9]){0,1}[0-9])|" +
                        "([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|" +
                        "1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|" +
                        "(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))/(((1)([0-2])([0-8]))|(([1-9])([0-9]))|([0-9])))$", RegexOptions.IgnoreCase)));
        _listCalculator.Add((SplitSubnetsByCountInterfaces, new Regex("^((([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|" +
                        "([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|" +
                        "([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|" +
                        "([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|" +
                        "([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|" +
                        "([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|" +
                        "[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|" +
                        ":((:[0-9a-fA-F]{1,4}){1,7}|:)|" +
                        "fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|" +
                        "::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|" +
                        "1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|" +
                        "1{0,1}[0-9]){0,1}[0-9])|" +
                        "([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|" +
                        "1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|" +
                        "(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))/(((1)([0-2])([0-8]))|(([1-9])([0-9]))|([0-9])))((\\s[0-9]{1,}){1,})(\\s*)$", RegexOptions.IgnoreCase)));
        _listCalculator.Add((Aggregation, new Regex("^((([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|" +
                        "([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|" +
                        "([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|" +
                        "([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|" +
                        "([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|" +
                        "([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|" +
                        "[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|" +
                        ":((:[0-9a-fA-F]{1,4}){1,7}|:)|" +
                        "fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|" +
                        "::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|" +
                        "1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|" +
                        "1{0,1}[0-9]){0,1}[0-9])|" +
                        "([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|" +
                        "1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|" +
                        "(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))/(((1)([0-2])([0-8]))|(([1-9])([0-9]))|([0-9]))\\s{0,}){2,}(\\s*)$", RegexOptions.IgnoreCase)));

    }

    public string Start()
    {
        return  "Здравствуйте, это бот калькулятор подсетей IPv6.\n" +
                "Чтобы узнать, что может этот бот и как им пользоваться напишите /help или нажмите соответствующую кнопку.\n" +
                "Всю информация о ipv6 можно получить здесь: https://ru.wikipedia.org/wiki/IPv6";
    }

    public string Help()
    {
        return "Что может этот бот? Он может делить, агрегировать и выдавать основную информацию о сети\n" +
            "Чтобы узнать подробнее о каждой возможности, нужно написать одну из следующих команд, либо нажать на соответствующую кнопку:\n" +
            "<b>как делить?</b> - получить информацию и пример о том, как правильно отправлять сеть для деления\n" +
            "<b>как соединять?</b> - получить информацию и пример о том, как правильно отправлять сети для агрегирования\n" +
            "<b>как делить по кол-ву интерфейсов?</b> - получить информацию и пример о том, как правильно отправлять сеть и количества интерфейсов для деления\n" +
            "<b>основная информация о сети</b> - получить информацию и пример о том, как правильно отправить сеть, чтобы получить информацию о ней\n";
    }


    public string DescSplitSubnets()
    {
        var s = "Напишите адрес IPv6 в следующем формате:\n" +
            "<b>address prefix new_prefix</b>, где\n" +
            "<b>address</b> - это адрес ipv6, записанный в соответствии с правилами сокращения.\n" +
            "Про правила сокращения можно узнать, перейдя по ссылке: https://studfile.net/preview/16437234/page:3/\n" +
            "<b>prefix</b> - это текущий префикс сети, который лежит в диапазоне 0 ⩽ prefix ⩽ 128\n" +
            "<b>new_prefix</b> - это новый префикс сети, который лежит в диапазоне prefix + 1 &lt new_prefix ⩽ 128\n" +
            "<b>ПРИМЕР ПРАВИЛЬНОГО ВВОДА</b> 2001:0db8::7a0:765d 18 20\n" +
            "После такого ввода вы получите 2^(new_prefix - prefix) сети(-ей)," +
            " т.е. если посмотреть на пример, то в результате получится 2^(20 - 18) = 4 сети\n" +
            "<b>ВАЖНО!</b> Этот бот может разделить сеть максимум на 4096 подсетей.";
        return s;
    }



    public string DescSplitSubnetsByCountInterfaces()
    {
        var s = "Для разделение сети на подсети по количеству интерфейсов нужно написать следующее:\n" +
            "<b>address/prefix count1 ... countN</b>, где\n" +
            "<b>address</b> - это адрес ipv6, записанный в соответствии с правилами сокращения.\n" +
            "Про правила сокращения можно узнать, перейдя по ссылке: https://studfile.net/preview/16437234/page:3/\n" +
            "<b>prefix</b> - это текущий префикс сети, который лежит в диапазоне 0 ⩽ prefix ⩽ 128\n" +
            "<b>count1 ... countN</b> - это список количеств интерфейсов, которые хотите иметь в каждой подсети\n" +
            "<b>ПРИМЕР ПРАВИЛЬНОГО ВВОДА</b> 2001:3200::/24 100 570 8\n"+
            "<b>ВАЖНО!</b> Этот бот может разделить сеть максимум на 4096 подсетей.";
        return s;
    }

    public string DescAddSubnets()
    {
        return "Напишите два или более адрес через пробел в слудующем формате:\n" +
            "<b>address1/prefix address2/prefix ... addressN/prefix</b>, где\n" +
             "<b>address</b> - это адрес ipv6, записанный в соответствии с правилами сокращения.\n" +
            "Про правила сокращения можно узнать, перейдя по ссылке: https://studfile.net/preview/16437234/page:3/\n" +
            "<b>prefix</b> - это текущий префикс сети, который лежит в диапазоне 0 ⩽ prefix ⩽ 128\n" +
            "<b>ПРИМЕР ПРАВИЛЬНОГО ВВОДА</b> 2001:3200::/24 2001:3300::/24\n";
    }

    public string DescriptionSubnet()
    {
        return "Чтобы получить основную информацию о сети, напишите любой адрес из этой сети в следующем формате:\n" +
            "addres/prefix, где\n" +
             "<b>address</b> - это адрес ipv6, записанный в соответствии с правилами сокращения.\n" +
            "Про правила сокращения можно узнать, перейдя по ссылке: https://studfile.net/preview/16437234/page:3/\n" +
            "<b>prefix</b> - это текущий префикс сети, который лежит в диапазоне 0 ⩽ prefix ⩽ 128\n" +
            "<b>ПРИМЕР ПРАВИЛЬНОГО ВВОДА</b> 2001:3200::/24";
    }

    public string DescriptionSubnet(string query)
    {
        string[] strings = query.Split('/');

        _ipv6.Parse(strings[0], Convert.ToInt32(strings[1]));

        ApplicationContext db = new ApplicationContext();
        var list = db.ReservedIPv6.Where(x => true).ToList();


        string response = "Информация о сети\n";


        var reservedIPv6 = list.Where(ip =>
        {
            return string.Compare(ip.StartAddres + "/" + ip.Prefix, _ipv6.toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(_ipv6.End, _ipv6.Slash)) >= 0;
        }).FirstOrDefault();

        if (reservedIPv6 != null)
        {
            response += $"<b>{IPv6Network.toString(_ipv6.Start, _ipv6.Slash)} является зарезервированной.</b>\n" +
                reservedIPv6.Description + ". " + reservedIPv6.Note + "\n";
        }

        response += $"Интервал:\n" +
            $"{IPv6Network.toString(_ipv6.Start, _ipv6.Slash)}\n" +
            $"• • •\n" +
            $"{IPv6Network.toString(_ipv6.End, _ipv6.Slash)}\n" +
            $"Количество интерфейсов {Math.Pow(2, 128 - _ipv6.Slash)}\n";

        return response;
    }

    public string SplitSubnets(string query)
    {
        string[] strings = query.Split(' ');
        int prefix = Convert.ToInt32(strings[1]);
        int newPrefix = Convert.ToInt32(strings[2]);

        if (prefix < MinPrefix || prefix > MaxPrefix)
            return $"Вы ввели недопустимое значение текущего префикса: prefix = {prefix}\n" +
                $"Его значение должно лежать в диапазоне {MinPrefix} ⩽ prefix ⩽ {MaxPrefix}";

        if (newPrefix <= prefix || newPrefix > MaxPrefix)
            return $"Вы ввели недопустимое значение нового префикса: new_prefix = {newPrefix}\n" +
                $"Его значение должно лежать в диапазоне {(prefix == MaxPrefix ? prefix : prefix + 1)} + 1 &lt new_prefix ⩽ {MaxPrefix}";

        _ipv6.Parse(strings[0], prefix);

        // база данных с зарезервированныйми адресами
        ApplicationContext db = new ApplicationContext();
        var listDB = db.ReservedIPv6.Where(x => true).ToList();
        var reservedIPv6 = listDB.Where(ip =>
        {
            return string.Compare(ip.StartAddres + "/" + ip.Prefix, _ipv6.toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(_ipv6.End, _ipv6.Slash)) >= 0;
        }).FirstOrDefault();

        if (reservedIPv6 != null)
            return $"<b>{_ipv6.toString()} является зарезервированной.</b>\n" +
                reservedIPv6.Description + ". " + reservedIPv6.Note + "\n";


        var list = _ipv6.SubnetsByNewSlashString(newPrefix);
        var response = $"{list.Count} новых подсети(-ей):\n";
        int lenhgthMessage = response.Length;
        string str = "";
        for (int i = 0; i < list.Count; i++)
        {
            reservedIPv6 = listDB.Where(ip =>
            {
                return string.Compare(ip.StartAddres + "/" + ip.Prefix, list[i]) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, list[i]) >= 0;
            }).FirstOrDefault();


            if (reservedIPv6 != null)
                str = $"{i + 1}) <b>{list[i]} является зарезервированной.</b>\n" + reservedIPv6.Description + ". " + reservedIPv6.Note + "\n";
            else
                str = $"{i + 1}) {list[i]}\n";

            if (str.Length + lenhgthMessage > _maxLengthMessage)
            {
                response += MessageSeparator;
                lenhgthMessage = 0;
            }
            else
                lenhgthMessage += str.Length;
            response += str;
        }
        return response;
    }



    public string SplitSubnetsByCountInterfaces(string query)
    {
        var strings = query.Split(" ");
        var address = strings[0].Split("/");
        _ipv6.Parse(address[0], Convert.ToInt32(address[1]));

        // база данных с зарезервированныйми адресами
        ApplicationContext db = new ApplicationContext();
        var listDB = db.ReservedIPv6.Where(x => true).ToList();
        var reservedIPv6 = listDB.Where(ip =>
        {
            return string.Compare(ip.StartAddres + "/" + ip.Prefix, _ipv6.toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(_ipv6.End, _ipv6.Slash)) >= 0;
        }).FirstOrDefault();
        
        if (reservedIPv6 != null)
            return $"<b>{_ipv6.toString()} является зарезервированной.</b>\n" +
                reservedIPv6.Description + ". " + reservedIPv6.Note + "\n";

        List<int> countInterfaces = new();
        for (int i = 1; i < strings.Length; i++)
            countInterfaces.Add(Convert.ToInt32(strings[i]));

        List<(string, string)> list;
        try
        {
            list = _ipv6.SubnetsByCountInterfaces(countInterfaces);
        }
        catch (Exception err)
        {
            return $"Поделить сеть {_ipv6.toString()} на подсети с задаными кол-вами интерфейсов нельзя. {err.Message}";
        }

        var response = $"{list.Count} новых подсети(-ей):\n";
        int lenhgthMessage = response.Length;
        string str = "";

        for (int i = 0; i < list.Count; i++)
        {

            reservedIPv6 = listDB.Where(ip =>
            {
                return string.Compare(ip.StartAddres + "/" + ip.Prefix, list[i].Item1) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, list[i].Item1) >= 0;
            }).FirstOrDefault();

            if (reservedIPv6 != null)
                str = $"{i + 1}) <b>{list[i].Item1} является зарезервированной.</b>\n" + reservedIPv6.Description + ". " + reservedIPv6.Note + "\n";
            else
                str = $"{i + 1}) {list[i].Item1} для {countInterfaces[i]} интерфейсов. Максимальное количество интерфейсов {list[i].Item2}\n";

            if (str.Length + lenhgthMessage > _maxLengthMessage)
            {
                response += MessageSeparator;
                lenhgthMessage = 0;
            }
            else
                lenhgthMessage += str.Length;
            response += str;
        }
        if (list.Count != countInterfaces.Count)
        {
            str = "Для некоторых количеств интерфейсов нельзя выделить подсеть, так как они являются зарезервированным.";
            if (str.Length + lenhgthMessage > _maxLengthMessage)
                response += MessageSeparator + str;
        }
            
        return response;
    }


    public string Aggregation(string query)
    {
        var strings = query.Split(" ");

        List<IPv6Network> addresses = new ();

        ApplicationContext db = new ApplicationContext();
        var listDB = db.ReservedIPv6.Where(x => true).ToList();

        for (int i = 0; i < strings.Length; i++)
        {
            var address = strings[i].Split("/");
            addresses.Add(new(address[0], Convert.ToInt32(address[1])));
            var reservedIPv6 = listDB.Where(ip =>
            {
                return string.Compare(ip.StartAddres + "/" + ip.Prefix, addresses[addresses.Count - 1].toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(addresses[addresses.Count - 1].End, addresses[addresses.Count - 1].Slash)) >= 0;
            }).FirstOrDefault();

            if (reservedIPv6 != null)
            {
                return $"<b>{addresses[addresses.Count - 1].toString()} является зарезервированной.</b>\n" +
                    reservedIPv6.Description + ". " + reservedIPv6.Note + "\n" +
                    "Удалите этот адрес из списка агрегируемых.";
            }
        }
        for (int i = 1; i < addresses.Count; i++)
            addresses[0] += addresses[i];

        
        

        /*if (reservedIPv6 != null)
        {
            return $"<b>{addresses[0].toString()} является зарезервированной.</b>\n" +
                reservedIPv6.Description + ". " + reservedIPv6.Note + "\n";
        }*/

        return addresses[0].toString();
    }

    public string GetAnswer(string query)
    {
        // убираем лишние пробелы
        query = Regex.Replace(query.Trim(), @"\s+", " ");

        for (int i = 0; i < _listCalculator.Count; i++)
        {
            if (_listCalculator[i].reg.IsMatch(query))
                return _listCalculator[i].f(query);
        }

        for (int i = 0; i < _list.Count; i++)
        {
            if (_list[i].reg.IsMatch(query))
                return _list[i].f();
        }

        return "Я ограничен в восприятии ваших запросов, именно поэтому пожалуйста пишите команды, которые я пойму😉\n" +
            "Чтобы узнать список команд, напишите /help или нажмите кнопу \"/help\"\n" +
            "<b>Совет:</b> если вы отправляли адрес(-са) IPv6, то проверьте его(их) написание, возможно он(они) написаны неправильно";
    }
}
