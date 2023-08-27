using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace BotTelega;

public class IPv6Network
{
    public BigInteger Address = BigInteger.Zero;
    public BigInteger Start = BigInteger.Zero; // первый адрес подсети
    public BigInteger End = BigInteger.Zero;   // последний адрес подсети
    public int Slash = 0; // префикс
    public long IndexSubnet = 0; // индекс подсети
    public BigInteger CountInterface = 0; // кол-во интерфейсов в сети

    private const int MaxSlash = 128;
    private int _slash = 0;
    private const int MaxDegree = 12; // 
    private const int CountBitInHexBlock = 32;
    private const int CountBitInHex = 4;
    private const int CountBitInByte = 8;
    private const int CountBlockInAddress = 8;
    

    public IPv6Network()
    {
        Address = BigInteger.Zero;
        Start = BigInteger.Zero;
        End = BigInteger.Zero;

        Slash = 0;
    }

    public IPv6Network(string addres, int slash) => Parse(addres, slash);


    public IPv6Network(BigInteger addres, int slash) => Parse(addres, slash);


    public void Parse(string address, int slash)
    {
        var addressParts = AddressParts(address);

        string s = "0";
        for (int i = 0; i < 8; i++)
            s += addressParts[i];

        Address = BigInteger.Parse(s, NumberStyles.AllowHexSpecifier);
        this.Slash = slash;
        StartEndAddresses();
    }

    public void Parse(BigInteger address, int slash)
    {
        Address = address;
        this.Slash = slash;
        StartEndAddresses();
    }


    // начальный и конечный адрес сети
    private void StartEndAddresses()
    {
        BigInteger mask = BigInteger.Parse("0ffffffffffffffffffffffffffffffff", NumberStyles.AllowHexSpecifier);

        mask = mask >> Slash;
        End = mask | Address;
        mask = ~mask;
        Start = mask & Address;

        CountInterface = (BigInteger)Math.Pow(2, MaxSlash - Slash);
        _slash = Slash;
        while (CountInterface <= 0)
        {
            _slash++;
            CountInterface = (BigInteger)Math.Pow(2, MaxSlash - _slash);
            if (_slash == MaxSlash)
                throw new Exception("Недостаточно оперативной памяти для рассчёта кол-ва интерфейсов");
        }
    }

    // разбивает address на блоки (по 16 бит) и возвращает их в виде списка
    private List<string> AddressParts(string address)
    {
        var AddressByBlocks = new List<string>();
        var partsAddress = address.Split(':');

        int index = -1;
        for (int i = 0; i < partsAddress.Length; i++)
        {
            if (partsAddress[i] == "" && index == -1)
            {
                index = i;
                continue;
            }
            else if (partsAddress[i] == "")
                continue;

            if (partsAddress[i].Length < CountBitInHex)
                while (partsAddress[i].Length != CountBitInHex)
                    partsAddress[i] = partsAddress[i].Insert(0, "0");

            AddressByBlocks.Add(partsAddress[i]);
        }


        if (AddressByBlocks.Count < CountBlockInAddress)
        {
            int count = AddressByBlocks.Count;
            for (int i = 0; i < CountBlockInAddress - count; i++)
                AddressByBlocks.Insert(index, "0000");
        }

        return AddressByBlocks;
    }

    // сокращает адресс, убирая из него не значащие нули и преобразуя первую группу ...0:0... в ::
    private static string ShortenIPv6Address(string address)
    {
        
        var addressParts = address.Split(":");

        for (int i = 0; i < addressParts.Length; i++)
        {
            while (addressParts[i].Length > 1 && addressParts[i][0] == '0')
                addressParts[i] = addressParts[i].Remove(0, 1);
        }

        // Замена нулевых групп на "::"
        int indexStart = -1;
        int indexEnd = -1;
        for (int i = 1; i < addressParts.Length; i++)
        {
            if (indexStart == -1 && addressParts[i-1] == "0" && addressParts[i] == "0")
            {
                indexStart = i - 1;
                indexEnd = i;
            }
            else if (indexStart != -1 && addressParts[i] == "0")
                indexEnd = i;
            else if (indexStart != -1 && addressParts[i] != "0")
                break;
        }

        string shortAddres = "";
        if (indexStart == -1)
        {
            for (int i = 0; i < addressParts.Length; i++)
                shortAddres += i < addressParts.Length - 1 ? addressParts[i] + ":" : addressParts[i];
            return shortAddres;
        }

        for (int i = 0; i < indexStart; i++)
        {
            shortAddres += i < indexStart - 1 ? addressParts[i] + ":" : addressParts[i];
        }
        shortAddres += "::";
        for (int i = indexEnd + 1; i < addressParts.Length; i++)
            shortAddres += i < addressParts.Length - 1 ? addressParts[i] + ":" : addressParts[i];

        return shortAddres;
    }

    // возвращает начальный адрес подсети в формате addres/slash
    public string toString()
    {
        if (Start == 0)
            return $"::/{Slash}";

        string s = String.Format("{0:x}", Start);
        while (s.Length > CountBitInHexBlock)
            s = s.Remove(0, 1);
        while (s.Length < CountBitInHexBlock)
            s = "0" + s;

        string result = "";
        for (int i = 0; i < s.Length; i++)
        {
            result += s[i];
            if ((i + 1) % CountBitInHex == 0 && i != s.Length - 1)
                result += ":";
        }
        result = result.Replace("0000", "0");
        result = ShortenIPv6Address(result) +  $"/{Slash}";
        return result;
    }


    public static string toString(BigInteger addres, int slash)
    {
        if (addres == 0)
            return $"::/{slash}";

        string s = String.Format("{0:x}", addres);
        while (s.Length > CountBitInHexBlock)
            s = s.Remove(0, 1);
        while (s.Length < CountBitInHexBlock)
            s = "0" + s;

        string result = "";
        for (int i = 0; i < s.Length; i++)
        {
            result += s[i];
            if ((i + 1) % CountBitInHex == 0 && i != s.Length - 1)
                result += ":";
        }
        result = result.Replace("0000", "0");
        result = ShortenIPv6Address(result) + $"/{slash}";
        return result;
    }

    // разделение сети на подсети, используя новое большее чем старое значение префикса
    // сеть делится на равные части
    // возвращает список объектов IPV6Address
    public List<IPv6Network> SubnetsByNewSlash(int newSlash)
    {
        List<IPv6Network> result = new();

        BigInteger mask = BigInteger.Parse("0ffffffffffffffffffffffffffffffff", NumberStyles.AllowHexSpecifier);
        mask = mask >> newSlash;
        BigInteger subnet = Start & ~mask;

        result.Add(new (subnet, newSlash));

        int degree = (newSlash - Slash) <= MaxDegree ? (newSlash - Slash) : MaxDegree;
        int count = (int)Math.Pow(2, degree);

        for (int i = 1; i < count; i++)
        {
            result.Add(new((subnet | mask) + BigInteger.One, newSlash));
            result[i].IndexSubnet = i;
            subnet = (subnet | mask) + BigInteger.One;
        }

        return result;
    }

    // разделение сети на подсети, используя новое большее чем старое значение префикса
    // сеть делится на равные части
    // возвращает список строк, подсетей
    public List<string> SubnetsByNewSlashString(int newSlash)
    {
        List<string> result = new();

        BigInteger mask = BigInteger.Parse("0ffffffffffffffffffffffffffffffff", NumberStyles.AllowHexSpecifier);
        mask = mask >> newSlash;
        BigInteger subnet = Start & ~mask;

        result.Add(toString(subnet, newSlash));

        int degree = (newSlash - Slash) <= MaxDegree ? (newSlash - Slash) : MaxDegree;
        int count = (int)Math.Pow(2, degree);
        
        for (int i = 0; i < count - 1; i++)
        {
            result.Add(toString((subnet | mask) + BigInteger.One, newSlash));
            subnet = (subnet | mask) + BigInteger.One;
        }

        return result;
    }

   /* private bool SubnetIsReservation(IPv6Network subnet, List<ModelReservedIPv6> listDB)
    {
        var reservedIPv6 = listDB.Where(ip =>
        {
            return string.Compare(ip.StartAddres + "/" + ip.Prefix, subnet.toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(subnet.End, subnet.Slash)) >= 0;
        }).FirstOrDefault();

        if (reservedIPv6 != null)
            return true;
        else
            return false;
    }*/

    // 
    public IPv6Network FirstSubnetByCountInterfaces(int CountInterface)
    {
        if (CountInterface > this.CountInterface)
            throw new Exception("Нельзя поделить");

        int countBit = (int)Math.Ceiling(Math.Log(CountInterface, 2));
        countBit = countBit == 0 ? 1 : countBit;
        int newSlash = MaxSlash - countBit;

        if (newSlash < Slash)
            throw new Exception("Error");

        BigInteger mask = BigInteger.Parse("0ffffffffffffffffffffffffffffffff", NumberStyles.AllowHexSpecifier);
        mask = mask >> newSlash;
        BigInteger subnet = Start & ~mask;

        //subnet = (Start | mask) + BigInteger.One;

        IPv6Network result = new IPv6Network(subnet, newSlash);

        return result;
    }

    // разделение сети на подсети, исходя из требуемого кол-ва интерфейсов для каждой подсети
    public List<(string, string)> SubnetsByCountInterfaces(List<int> countInterfaces)
    {
        countInterfaces.Sort((x, y) => y.CompareTo(x));

        // блок проверок
        BigInteger checkSum = 0;
        int i;
        for (i = 0; i < countInterfaces.Count; i++)
        {
            int j = (int)Math.Ceiling(Math.Log(countInterfaces[i], 2));
            j = j == 0 ? j + 1 : j;
            checkSum += (BigInteger)Math.Pow(2, j);
        }
        List<(string, string)> result = new();
        if (checkSum > this.CountInterface)
            throw new Exception("Cумма количеств интерфейсов превышает вместимость исходной сети.");

        ApplicationContext db = new ApplicationContext();
        var listDB = db.ReservedIPv6.Where(x => true).ToList();

        i = 0;
        IPv6Network subnet = FirstSubnetByCountInterfaces(countInterfaces[i]);
        int CurrentMinSlash = subnet.Slash;

        while (true)
        {
            var reservedIPv6 = listDB.Where(ip =>
            {
                return string.Compare(ip.StartAddres + "/" + ip.Prefix, subnet.toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(subnet.End, subnet.Slash)) >= 0;
            }).FirstOrDefault();

            if (reservedIPv6 != null)
            {
                try
                {
                    IPv6Network dopIPv6 = new(reservedIPv6.StartAddres, reservedIPv6.Prefix);
                    subnet = SubnetByLastPrev(subnet.Slash, dopIPv6.End);
                }
                catch
                {
                    throw new Exception("Все подсети для данных количеств являются зарезервированными.");
                }
            }
            else
                break;
        }
        i++;
        result.Add(new(subnet.toString(), subnet.CountInterface.ToString()));
        BigInteger last;
        while (i < countInterfaces.Count)
        {
            last = subnet.End;
            subnet = FirstSubnetByCountInterfaces(countInterfaces[i]);
            if (CurrentMinSlash == subnet.Slash)
            {
                subnet = SubnetByLastPrev(subnet.Slash, last);
                while (true)
                {
                    var reservedIPv6 = listDB.Where(ip =>
                    {
                        return string.Compare(ip.StartAddres + "/" + ip.Prefix, subnet.toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(subnet.End, subnet.Slash)) >= 0;
                    }).FirstOrDefault();

                    if (reservedIPv6 != null)
                    {
                        try
                        {
                            IPv6Network dopIPv6 = new(reservedIPv6.StartAddres, reservedIPv6.Prefix);
                            subnet = SubnetByLastPrev(subnet.Slash, dopIPv6.End);
                        }
                        catch
                        {
                            goto End;
                        }
                    }
                    else
                        break;
                }
            }
            else
            {
                subnet = SubnetByLastPrev(subnet.Slash, last);
                while (true)
                {
                    var reservedIPv6 = listDB.Where(ip =>
                    {
                        return string.Compare(ip.StartAddres + "/" + ip.Prefix, subnet.toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(subnet.End, subnet.Slash)) >= 0;
                    }).FirstOrDefault();

                    if (reservedIPv6 != null)
                    {
                        try
                        {
                            IPv6Network dopIPv6 = new(reservedIPv6.StartAddres, reservedIPv6.Prefix);
                            subnet = SubnetByLastPrev(subnet.Slash, dopIPv6.End);

                        }
                        catch
                        {
                            goto End;
                        }
                    }
                    else
                        break;
                }
                CurrentMinSlash = subnet.Slash;
            }

            result.Add(new(subnet.toString(), subnet.CountInterface.ToString()));
            End:
            i++;
        }

        return result;
    }

    public IPv6Network SubnetByLastPrev(int newSlash, BigInteger last)
    {
        if (newSlash < Slash || last == End)
            throw new Exception("Error");

        BigInteger subnet = last + BigInteger.One;
        IPv6Network result = new IPv6Network(subnet, newSlash);

        return result;
    }


    // агрегация двух сетей
    public static IPv6Network operator +(IPv6Network ipOne, IPv6Network ipTwo)
    {

        var bitOne = new BitArray(ipOne.Start.ToByteArray());
        var bitTwo = new BitArray(ipTwo.Start.ToByteArray());

        int minSlash = ipOne.Slash < ipTwo.Slash ? ipOne.Slash : ipTwo.Slash;
        var bitAggregation = new BitArray(129);
        int newSlash = 0;

        for (int i = bitAggregation.Length - 2; i >= 0; i--)
        {
            if (bitOne[i] == bitTwo[i])
            {
                bitAggregation[i] = bitTwo[i];
                newSlash++;
                if (newSlash > minSlash)
                {
                    newSlash = minSlash;
                    break;
                }
            }
            else
                break;
        }

        byte[] byteArray = new byte[bitAggregation.Length / CountBitInByte + 1];
        bitAggregation.CopyTo(byteArray, 0);


        IPv6Network result = new(new BigInteger(byteArray), newSlash);
        return result;
    }

}





// разделение сети на подсети, исходя из требуемого кол-ва интерфейсов для каждой подсети
/*public List<(string, string)> SubnetsByCountInterfaces(List<int> countInterfaces)
{
    countInterfaces.Sort((x, y) => y.CompareTo(x));

    // блок проверок
    BigInteger checkSum = 0;
    int i;
    for (i = 0; i < countInterfaces.Count; i++)
    {
        int j = (int)Math.Ceiling(Math.Log(countInterfaces[i], 2));
        j = j == 0 ? j + 1 : j;
        checkSum += (BigInteger)Math.Pow(2, j);
    }
    List<(string, string)> result = new();
    if (checkSum > this.CountInterface)
        throw new Exception("Cумма количеств интерфейсов превышает вместимость исходной сети.");

    ApplicationContext db = new ApplicationContext();
    var listDB = db.ReservedIPv6.Where(x => true).ToList();


    i = 0;
    IPv6Network subnet = SubnetByCountInterfacesAndIndex(countInterfaces[i]);
    long indexNextSubnet = 1;
    int CurrentMinSlash = subnet.Slash;

    while (true)
    {
        var reservedIPv6 = listDB.Where(ip =>
        {
            return string.Compare(ip.StartAddres + "/" + ip.Prefix, subnet.toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(subnet.End, subnet.Slash)) >= 0;
        }).FirstOrDefault();

        if (reservedIPv6 != null)
        {
            try
            {
                indexNextSubnet += (int)Math.Pow(2, subnet.Slash - reservedIPv6.Prefix) - 1;
                subnet = SubnetByCountInterfacesAndIndex(countInterfaces[i], indexNextSubnet);
                indexNextSubnet++;
            }
            catch
            {
                throw new Exception("Все подсети для данных количеств являются зарезервированными.");
            }
        }
        else
            break;
    }
    i++;
    result.Add(new(subnet.toString(), subnet.CountInterface.ToString()));



    while (i < countInterfaces.Count)
    {
        subnet = SubnetByCountInterfacesAndIndex(countInterfaces[i]);

        if (CurrentMinSlash == subnet.Slash)
        {
            subnet = SubnetByCountInterfacesAndIndex(countInterfaces[i], indexNextSubnet);
            indexNextSubnet++;

            var reservedIPv6 = listDB.Where(ip =>
            {
                return string.Compare(ip.StartAddres + "/" + ip.Prefix, subnet.toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(subnet.End, subnet.Slash)) >= 0;
            }).FirstOrDefault();

            while (reservedIPv6 != null)
            {
                try
                {
                    indexNextSubnet += (int)Math.Pow(2, subnet.Slash - reservedIPv6.Prefix) - 1;
                    subnet = SubnetByCountInterfacesAndIndex(countInterfaces[i], indexNextSubnet);
                    indexNextSubnet++;
                }
                catch
                {
                    goto End;
                }

                reservedIPv6 = listDB.Where(ip =>
                {
                    return string.Compare(ip.StartAddres + "/" + ip.Prefix, subnet.toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(subnet.End, subnet.Slash)) >= 0;
                }).FirstOrDefault();
            }
        }
        else
        {
            indexNextSubnet--;
            indexNextSubnet = (int)(indexNextSubnet * Math.Pow(2, subnet.Slash - CurrentMinSlash) + Math.Pow(2, subnet.Slash - CurrentMinSlash));
            subnet = SubnetBySlashAndIndex(CurrentMinSlash, indexNextSubnet);
            indexNextSubnet++;

            var reservedIPv6 = listDB.Where(ip =>
            {
                return string.Compare(ip.StartAddres + "/" + ip.Prefix, subnet.toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(subnet.End, subnet.Slash)) >= 0;
            }).FirstOrDefault();

            while (reservedIPv6 != null)
            {
                try
                {
                    indexNextSubnet += (int)Math.Pow(2, subnet.Slash - reservedIPv6.Prefix) - 1;
                    subnet = SubnetByCountInterfacesAndIndex(countInterfaces[i], indexNextSubnet);
                    indexNextSubnet++;
                }
                catch
                {
                    goto End;
                }

                reservedIPv6 = listDB.Where(ip =>
                {
                    return string.Compare(ip.StartAddres + "/" + ip.Prefix, subnet.toString()) <= 0 && string.Compare(ip.EndAddres + "/" + ip.Prefix, IPv6Network.toString(subnet.End, subnet.Slash)) >= 0;
                }).FirstOrDefault();
            }

            CurrentMinSlash = subnet.Slash;

        }


        result.Add(new(subnet.toString(), subnet.CountInterface.ToString()));
        i++;
    }
    End:
    return result;
}

// 
public IPv6Network SubnetByCountInterfacesAndIndex(int CountInterface, long indexSubnet = 0)
{
    if (CountInterface > this.CountInterface)
        throw new Exception("Нельзя поделить");

    int countBit = (int)Math.Ceiling(Math.Log(CountInterface, 2));
    countBit = countBit == 0 ? 1 : countBit;
    int newSlash = MaxSlash - countBit;

    if (newSlash < Slash || indexSubnet >= Math.Pow(2, newSlash - Slash))
        throw new Exception("Error");

    BigInteger mask = BigInteger.Parse("0ffffffffffffffffffffffffffffffff", NumberStyles.AllowHexSpecifier);
    mask = mask >> newSlash;
    BigInteger subnet = Start & ~mask;

    for (int i = 1; i <= indexSubnet; i++)
    {
        subnet = (subnet | mask) + BigInteger.One;
    }

    IPv6Network result = new IPv6Network(subnet, newSlash);
    result.IndexSubnet = indexSubnet;

    return result;
}

//
public IPv6Network SubnetBySlashAndIndex(int newSlash, long indexSubnet = 0)
{
    BigInteger mask = BigInteger.Parse("0ffffffffffffffffffffffffffffffff", NumberStyles.AllowHexSpecifier);
    mask = mask >> newSlash;
    BigInteger subnet = Start & ~mask;

    if (newSlash < Slash || indexSubnet >= Math.Pow(2, newSlash - Slash))
        throw new Exception("Error");

    for (int i = 1; i <= indexSubnet; i++)
        subnet = (subnet | mask) + BigInteger.One;

    IPv6Network result = new IPv6Network(subnet, newSlash);
    result.IndexSubnet = indexSubnet;

    return result;
}*/