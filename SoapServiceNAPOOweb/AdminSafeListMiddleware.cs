using ISNAPOO.Common.HelperClasses;
using System.Linq;
using System.Net;

namespace SoapServiceNAPOOweb
{
    public class AdminSafeListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminSafeListMiddleware> _logger;
        private readonly byte[][] _azSafelist;
        private readonly byte[][] _webServiceSafelist;
        private readonly byte[][] _egovSafelist;


        private readonly string AZSafelist;
        private readonly string WebServiceSafelist;
        private readonly string EgovSafelist;
        
        private readonly List<Tuple<string, string, string>> EgovSafeListRange = new List<Tuple<string, string, string>>();

        public AdminSafeListMiddleware(
            RequestDelegate next,
            ILogger<AdminSafeListMiddleware> logger,
            string azSafelist,
            string webServiceSafeList,
            string egovSafeList,
            string egovSafeListRange
            )
        {

            this.AZSafelist = azSafelist;
            this.WebServiceSafelist = webServiceSafeList;
            this.EgovSafelist = egovSafeList;
            



            string[] ips = azSafelist.Split(';');
            _azSafelist = new byte[ips.Length][];
            for (var i = 0; i < ips.Length; i++)
            {
                _azSafelist[i] = IPAddress.Parse(ips[i]).GetAddressBytes();
            }


            ips = null;

            ips = webServiceSafeList.Split(';');
            _webServiceSafelist = new byte[ips.Length][];
            for (var i = 0; i < ips.Length; i++)
            {
                _webServiceSafelist[i] = IPAddress.Parse(ips[i]).GetAddressBytes();
            }

            ips = null;

            ips = egovSafeList.Split(';');
            _egovSafelist = new byte[ips.Length][];
            for (var i = 0; i < ips.Length; i++)
            {
                _egovSafelist[i] = IPAddress.Parse(ips[i]).GetAddressBytes();
            }




            
            foreach (var range in egovSafeListRange.Split(';'))
            { 
                EgovSafeListRange.Add(new Tuple<string, string, string>(range, range.Split('-')[0].Trim(), range.Split('-')[1].Trim()));
            }

            _next = next;
            _logger = logger;

            
            

        }

        public async Task Invoke(HttpContext context)
        {

            //if (context.Request.Method != HttpMethod.Post.Method)
            //{

            //    _logger.LogWarning("Forbidden Request ");
            //    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            //    return;

            //}
            if (context.Request.Method == HttpMethod.Post.Method)
            {
                var remoteIp = context.Connection.RemoteIpAddress;

                string strRemoteIp = remoteIp.ToString();



                _logger.LogInformation($"Request from Remote IP address: {remoteIp}" );
                _logger.LogInformation($"Context.Request.Path : {context.Request.Path.ToString()}");

                var bytes = remoteIp.GetAddressBytes();
                var badIp = true;


                if (context.Request.Path.ToString().Contains("AZService.asmx"))
                {
                    _logger.LogInformation($"Allow IPS for AZSafelist : {AZSafelist}");

                    foreach (var address in _azSafelist)
                    {
                        if (address.SequenceEqual(bytes))
                        {
                            badIp = false;
                            break;
                        }
                    }

                    if (badIp)
                    {
                        _logger.LogWarning("Forbidden Request from Remote IP address: {RemoteIp} for AZService", remoteIp);
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return;
                    }

                }
                else if (context.Request.Path.ToString().Contains("EGOVService.asmx"))
                {

                    _logger.LogInformation($"Allow IPS for EgovSafelist : {EgovSafelist}"); 
                    _logger.LogInformation($"Allow IPS for EgovSafeListRange : {string.Join(';', EgovSafeListRange.Select(x => x.Item1))}");

                    foreach (var address in _egovSafelist)
                    {
                        if (address.SequenceEqual(bytes))
                        {
                            badIp = false;
                            break;
                        }
                    }

                    if (badIp) 
                    {
                        foreach (var range in EgovSafeListRange) 
                        {
                            if (ValidateIPRange(range.Item2, range.Item3, strRemoteIp)) 
                            {
                                badIp = false;
                                break;
                            }
                        }
                    }

                    if (badIp)
                    {
                        _logger.LogWarning(
                            "Forbidden Request from Remote IP address: {RemoteIp} for EgovSafe", remoteIp);
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return;
                    }

                }
                else if(context.Request.Path.ToString().Contains("Service.asmx"))
                {
                    _logger.LogInformation($"Allow IPS for WebServiceSafelist : {WebServiceSafelist}");
                    foreach (var address in _webServiceSafelist)
                    {
                        if (address.SequenceEqual(bytes))
                        {
                            badIp = false;
                            break;
                        }
                    }

                    if (badIp)
                    {
                        _logger.LogWarning("Forbidden Request from Remote IP address: {RemoteIp} for web service", remoteIp);
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return;
                    }

                }
                
                else
                {
                    _logger.LogWarning(
                            "Forbidden Request from Remote IP address: {RemoteIp}", remoteIp);
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }


            }

            await _next.Invoke(context);
        }

        public bool ValidateIPRange(string startIP, string endIP, string targetIP)
        {
            _logger.LogDebug($"startIP:{startIP} endIP:{endIP} targetIP:{targetIP}");

            IPAddress parsedStartIP, parsedEndIP, parsedTargetIP;
            if (IPAddress.TryParse(startIP, out parsedStartIP) && IPAddress.TryParse(endIP, out parsedEndIP) && IPAddress.TryParse(targetIP, out parsedTargetIP))
            {
                //long startIPValue = BitConverter.ToInt32(parsedStartIP.GetAddressBytes(), 0);
                //long endIPValue = BitConverter.ToInt32(parsedEndIP.GetAddressBytes(), 0);
                //long targetIPValue = BitConverter.ToInt32(parsedTargetIP.GetAddressBytes(), 0);

                //return targetIPValue >= startIPValue && targetIPValue <= endIPValue; // Проверява дали целевият IP адрес е в зададения диапазон


                _logger.LogDebug($"parsedStartIP:{parsedStartIP} parsedEndIP:{parsedEndIP} parsedTargetIP:{parsedTargetIP}");

                uint ipValue = IPToUInt(parsedTargetIP);
                uint startValue = IPToUInt(parsedStartIP);
                uint endValue = IPToUInt(parsedEndIP);

                
                _logger.LogDebug($"ipValue:{ipValue} startValue:{startValue} endValue:{endValue}");

                // Проверяваме дали ipValue е между startValue и endValue
                return ipValue >= startValue && ipValue <= endValue;
            }
            else
            {
                return false; // Някой от IP адресите не е валиден
            }
        }
        public uint IPToUInt(IPAddress ip)
        {
            // Вземаме байтовете на IP адреса
            byte[] bytes = ip.GetAddressBytes();

            // Обръщаме реда на байтовете, ако сме на little-endian система
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            // Преобразуваме байтовете в цяло число
            uint value = BitConverter.ToUInt32(bytes, 0);

            // Връщаме резултата
            return value;
        }


    }


}
