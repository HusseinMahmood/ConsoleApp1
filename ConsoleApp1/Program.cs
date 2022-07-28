using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using Leaf.xNet;
using TwoCaptcha.Captcha;

namespace ConsoleApp1
{
    internal class Program
    {
        static List<string> vs    = new List<string>();
        static List<string> proxy = new List<string>();
        static Random r = new Random();
        static int item = 0;
        static string RandomToken()
        {
            string[] tokens = File.ReadAllLines("token.txt");
            int z = item++ % tokens.Length;
            return tokens[z];
        }
        static string randomString(int len)
        {
            string output = "";
            for (int i = 0; i < len; i++)
            {
                output += (char)r.Next(97, 122);
            }
            return output;
        }
        static string randomNum(int len)
        {
            string output = "";
            for (int i = 0; i < len; i++)
            {
                output += r.Next(0, 10).ToString();
            }
            return output;
        }

        static void Main(string[] args)
        {


            //string res = File.ReadAllText("web.txt");//GetRequest("https://forebears.io/iraq/forenames");
            //MatchCollection match = Regex.Matches(res,"<a href=\"forenames/(.*?)\">(.*?)</a>");

            //foreach(Match m in match)
            //{
            //    File.AppendAllText("name.txt", m.Groups[1].Value + "\r\n");
            //}
            //new Thread(() => checkproxy()).Start();



            new Task(() => { checkproxy(); }).Start();

            Thread.Sleep(10000);
            GetIdes();




            string[] names = File.ReadAllLines("name.txt");
            foreach(string name in names)
                Search(name);

            for (int i = 0; i < 50; i++)
                Search(randomString(3));


            Register();
            // https://api.tellonym.me/suggestions/people?oldestId=&adExpId=8&limit=50&pos=30

            Console.ReadLine();
        }

        static string Base64_Text(string data)
        {
            try
            {

                byte[] d = System.Convert.FromBase64String(data);
                return System.Text.ASCIIEncoding.UTF8.GetString(d);

            }
            catch
            {
                try
                {
                    byte[] d = System.Convert.FromBase64String(data + "=");
                    return System.Text.ASCIIEncoding.UTF8.GetString(d);
                }
                catch
                {
                    try { 
                        byte[] d = System.Convert.FromBase64String(data + "==");
                        return System.Text.ASCIIEncoding.UTF8.GetString(d);
                    }
                    catch { return ""; }

                }
            }
        }

        static void Search(string keyword)
        {
            string ak = File.ReadAllText("id.txt");

            string limit = "50";
            string url = $"https://api.tellonym.me/search/users?searchString={keyword}&term={keyword}&adExpId=8&limit={limit}";
            string res = GetRequest(url);

            MatchCollection matches = Regex.Matches(res, "\"id\":\"(.*?)\",");

            foreach (Match m in matches)
            {

                string id = m.Groups[1].Value;
                id = Base64_Text(id).Split(':')[1];
                
                if (!vs.Contains(id) && !ak.Contains(id))
                {
                    vs.Add(id);
                    SendMsg(id);
                    File.AppendAllText("id.txt", id + "\r\n");
                    Thread.Sleep(15000);
                }
            }


        }

        static string GetCaptcha()
        {

            return "P0_eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwYXNza2V5IjoiNEVPcFVYOGh6SVlaUitIRHErM3dPWDFpRzNxQVRiSU5yTHUvZkpZelFZeUZYRDZYRUF1RkhCbzZRTCs5VkdaME0yY3gwZGhUSVdpQThhTWhlblRuTlQ0dEliVlE3L0R0T1VlNXlQOERrK1RwdVV4RDRWTTMrTXZ3bm9RNHd6R3E2NmZkcGV1eEhjN3Z6SXphVHV5QVlHdWozVXZlRnJuRmZ6bm1PMUZTdUxzNVhyUnJBazFWZTVuc2x4ZjYvM01ZRk1yWlFBYmFRdjlEWFd2cUFORTVEK0djSFhQdFI3VU1hZmhrTVZYSVN6dHczWEVtZEZOeC8vZGZMSzVvZTREbXk0b0ZjcitlZ0JJWStRckhQdUlsMDdCWDhaQSsrUXZJUFFKMnVKblUrRVNKc2pyRHlQUVFWKzNVRXBqRlptS3pZZUg5dDFNdVZvckF1S3Z2T1BjcWZ4VHNYTE0wWW5pYXZ6bVllMThOK1YvYWN2aE91dDJLUFpQT1pkWTNFUUdXQ2d6UTlHblFDcytIRHRuSE9RT3ZZbjF0VlNSQW5sWkozaFlyejFNd2dvY1dZZDhVWFJmRHNlL1BlSzVYSFBrcXVnQ250a2h3ODNmSUZtUmRNZDJFMS9SZ1pQQlRLV0FITk13YkZnWDNYeUF0N0pDMlo2TDVrcTRLNmxQakdmRmlnWUJkM1V4dkpFZ3ZnOU1mSjRHaVFXNkkwdEhXRENSY0JodFhGanIrdzJMTklaNlhRd2x6UEUzQ2RIZzZid1I3NUMzOG93dmVsZ3hqZUU2SDR0RUdlOHdsbjEyRVp6ckFSSXd1anJ4M0FlZ3llclZGRXZrSVBnUU81ZHY5cnp0Uk1yVWFPZmM4bUQxbDhpUDRFamlZS3V3cjlmangzOHlQYlBYR0JmYUtTTERlNFJ3TzNzem5JWmliZHlucG5NaWhFWjZna2FRdmZmVVNFYkNtMEUvT3dsUi9VYmQrelZGaUZFK2RMbzZVVFNVVDFNRWEyOVcraDVYdXR6OG01eDVsV013VGxuRFNNK3FIUjdqVmkzSk5CRlJIUnFPa3FtZ3IwaEhUQ29TcWNLKytPcnQ3VDMrcDNFc2lHaVVhd3BjZ0NUVkZKcTNUTDdTUnNFcFdxZHpUZG0xQmEyWjZFY0sra1hOZnFPS0F1OHU4bTFiSStYMlVpWWl1eDFwSlY2bHZGcWlXdFVUdlZqL0Q1clpyQkpycVhWdFhxTS9pNFpoZVRyMm5ZSkZvQ2h0NHlIWktuQXBKVC9XVlF2dCtQM3NuN1VCM0lGbVdIdUNxVGtZMTZJb0FwZzBHMkFrSlV3cUM3VERESmNXRVAyUGNSSmFrRmlwK1dLeFdGOXR3Q09WdW5nblp1ZCtYb01hUVIrZUJ2REhSS0lxOHp6Q3UrWURObU4yeTgxWnFvQmhwRTdUU21oZ0VPeXI4YkxqSEd3aHNodTdiT25jeWhiUFBJejJFUTZ0b3V5SWxtRnhwVWE3RmduNEdXZ082WHMreWNVRWxsTmd1eXRGQk41VzZIaGlERzJwQkVhNHhNSzVLNFJkemR2T0F2UlV5a3lVbWdYTnRYWGlHcUtEUTN0a1dqMFIyNmo2QmRHMmxWaDFkb3dpRTRWNDYzeC9ZUjhTZG5SLytPSHBrYjZWR0xrR1NvR0tZSlJ0M0J6ZHhjSlpsaElONmFERXYzRjhibGd6Wm1lL0o5WnpaNWVXM3ZXN2pkMnNOOHRpQU5TRkF6eHo4TStEdnMzUm55RHRBM2JKY3hqYkozUTlWMHRIa2tXV3A2WjFqRURNUW5sQ1U3clNaanBjS3YwL2RYek9UWTJRcDEya05NWmF3SmlYcGxiWnVjbjhhUEZSNTdWQmRYbnY5NmJ1TktTaFdQcUpiakppMXZvZjdQOExIa3p1YkZ2TmJ0NnRzYkVCeG8rZnMxdGFSWTJXWnlwM3l5WDJzSEF1UElReC9Pd1pZZWlIU0U3MkdrNGRqdUl3d1VxNjY2R3JnSWJuRlJiTk5hNEhsRjN2eXZyUmhUMnV4cm44dnpaM1VOZWZXOW9YbzA4aFE3VHdiejQ1MlRJZEhxWVVTU3Q1THdmWXIzNHdkaFJkaFhXenRkUUZtdTl0MzBzM1VBblA1djE5L1VFOS9vSVZKSDFJQWVRYXdzMDkxdDFoRHNOcGx4Ym5JVUwrdnBENzcxWU9GOUdLdVJWVDVXS0tSWlR1Z0xqRXhWaDlIUnRDYTBTdnl0NlRFMzFwSVNlMnd1NWRVOEh5aVRXV0NrVWtGdlFjd0lIV0dTUHRGQUVLcDZUYldnc1lDTjRNaVNibHRFaW45OFB5cDV6d0xjOVFEMFZ0SG01MnJMNmRvVGU2SGUxY2lDbTg3cldjMEQ5ZHZRdkl5S2Y4UW9RNU4wWE9rZWNQbkE1cG0xenNRWHVlZUU1VDRNTjYxbUlyQVJPTHovNUJXNUZWenJaZjZ5elZNcmpTdmhhQ3Rwclg2eW1VMWRPRUpYVTFkVHZNWWpaZjhZZWl5R3dtUU95aHBJN09kNUNFS2NRa25ubFZWOFduV2UxSXNEV3V3eUV5RlBtdndURXNsdkg4UDZkQW4vWUdNOHZzdTZFZFlZYkY2REN5NHNxQjVUaXliZU9LYWtOUXlYampISDY2QzVESHJ1eVZsY0NBRjFsZzd3WE5jUDI3QjUrVWF4SVVxejRPaXRwaHovSE1tNlFacjNWUW1LUlFEaDZlNFA0R3RXVkdtRDZwd3U4a0d0Tm9CNjFPalBKTDJBcnBmZVNDcVh5OHlYSThFUXdNYVpzdjNxaTFMVUp6NkJ5WUUrVTYrQTNray93PT1wcnR3dGhzWDIwT2Q3cGRHIiwiZXhwIjoxNjQ5MzA2MjU1LCJzaGFyZF9pZCI6ODIwNzg2MDg2LCJwZCI6MH0.HPFIaqJjBawqkjdNyl2QxXG0Y7a7fzdS8Joqdx5fZMM";


            TwoCaptcha.TwoCaptcha solver = new TwoCaptcha.TwoCaptcha("449cfe7c13d2fe0f98b01392f63759e4");

            HCaptcha captcha = new HCaptcha();
            captcha.SetSiteKey("3e48b1d1-44bf-4bc4-a597-e76af6f3a260");
            captcha.SetUrl("https://hhsen.ml/Kasper/tell.html");

            try
            {
                solver.Solve(captcha).Wait();
                return captcha.Code;
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Error occurred: " + e.InnerExceptions.First().Message);
            }

            //return "P0_eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwYXNza2V5IjoiS00xWWVuVTdNTVFFU0tOMFNwREl1WXdWc0E0NXlEUFlJdm1rbXVZakhKTlRucW5jeWU0WUp4NXBYME5wRE9BRlNFQTFzM1A3b3lGRm5uWWRVTjZCZ1VQWDJzdEdZeGFpYWZ2eUVaNndqS2M1YXo3eUxFemI2YVlTN1d3ZXp5NWRvaUhwdXc5MVYyTEVONG9zb3Z4TVNJNm1WWm9UeTlNYVk5cnpBVmJRNWtlTmJ0cUxRbEhxL201MXJmeWFzWlhNWkVCdjlJdEVPbGltMjJJTTh1MjlVbFc3VjNlQkdzUzhSbEZ5UXY1eEl0aXVFS2JRU054Z0VLZzZmVDQ5UEJ6ZXpTdkMxSmY3ZWxvUmlHM2xzWE5xYkhEckoyaEUrdlh2akJ2NXdncjZsUWp1TVp1cnpQZXRrN01BcUlqUjhESSs4QWpmUEk4T2laM2hjYUtDai91NkVwYzRPTnlVSFdWcmN6c2VSN1FrSTVCS0YybmFpQVF6aS9jckdSL0lPYitxQVJ1S0dZbjJTWnhUbjlQa1dzRDVZOCswY0F4cjUxTk5JRU93SSs2MmxkUlNhUkV5bWExQit5S2krU2ZtelUvbjZRdzB6OGlRWU1GR2R3RlcxN1gweG1EYUZjeXJWZXNsVVhuRlFjU0ZqbHNCeE5xWEpLNHdZVng3cUlzbldaQ29qbHZkd3hmMVBGK3BMWDcxcWxhaFZseXJVQXNQZTlNem1jK2xBR2t1U3dmS3Njc2M3MkdiT1NHOHZRMVZQLzhpY0pGaytKby9hU2RMbVU2YW5ibktSZ2tsOGZqVUkxN3ptVG16blNxWnliODdFa1FJSzhxZTh2MCtENlVNdHBqRHhXMEthQ01nNmhSUlR0SGg2SEN3WVZrNkQ5TjU1ckRoWXRnRlB0WlZCaldZSG4vWXNhb0d6S2tuVFlYaVRHVkRwRUtXeE9CYWtXKytPa0FhTm83MTUvS1RxdzNGeUFPMzZ4QUd5ZUljQkhCbVptTnVGOGJhTTJoYVFnZThmQWFHN2dRYjVCT3FPOUxVSVpWTHlaRlkrK3NweGlnekl2UTdyN1VzVGlwV1hla09ybUdEVnBnaVoySjFJS3gyQTNlYVBpVEdYTjZwR1ZwakFoRUVFa1l5UWhJSEpKTnVtTWgwS1Ntc0pHbE9XdkloVVc1dFk2bldEM1ZvbVF1dmxMRmFQWmVjeFVTczN5cUFySjA0R0pXbWEwNmZMYVUyN0FycFE1UVUyVzFLL3F6bzc2cnFQTHFSMkpiT3BSMjN4UWRLVWRlV3hvUWk2aHVTMnpYeFVyOEFsWTJpQkkvV3luMFRZUW1odkE2TGx2UVV0MUoveUZHaFRFN3dVL1NPUjVPVjZYU0txam80SDcwNllaS2daMmQrR0RTQlBqai9CaXhzUDhBazlwNXE4ZVBOM0FUY2EzcGplOThsZGY1SUp1NGhqbzRkdU5ReFJOdWgvWVNyR0tlRWVHcWcxVm5KTnRYRy9Xdk1pL1hsalBZc2VOVTRRWDNYMUN5emY1Sy9RVldkMDZGNExaTnRBb3FuaENPdm5rZVYzbFgrT1lZcFJpL1NRcThEKzlmMk9GMG9iaUY1VEFNSnVJNmFhODgyd1I0NUlaUUs0UjRUR1Q2R3N5YjlWdENtS1V5UHUvR2VnZktaeTVuU0VhNVd6Y0p4MnVPZW8zOGRyNnRxUFdiSEVhRUIvRXpTUnhBcVRjNnVlN1VUVkVQYytoVHFMZG9YQUdGcXpIcVJGQ1dYWG1wKys1c2l4eEd5NzBObjR4Qis0czZOcm90RWgrY21FSUlUbzZRcXRDN2k0SHlMd0YrUU5oeVR4Rnl0MkY1VjZIdXVWWFU0NzJXbWtJL2tlSktuUjU4SVAzMTROb3h4T1dDYldYZUxPWHU1SmhSWnJ0cC9aTnlWRzVkUjZ0NTdRMXpEWHJ6V3JVaFE5dENjaVZsV1RWQ084cEt0cm0xUVlhZ1l4NGFuZXNCMWZUWE13TFZJZXFXbHNhSEg5L2dJUk1udWlRblM0Ri91NW92ZC9jUi93ZUZyb2VKY3Y0cXZDNkRVTk1ZZDlRVlJMMXhRc28yaGlMemxVWXFqc201T2NxQk5adWxCUnYrWmhkanM5YXd5VUkwT0JXczZMM3hPUXZNczNnTTdRU3ZPZUNTNlhIcTRFS1RtQ3FIR2oyQmFua3BwUlEzTlZpUndjb1ZYeFY0SWZwd3h0dVV3RE5BS0ZoTktKODZVVXJENDlCMzdxVE80UzdIdnN6QzJQVUFDM01vZzRZRnJFajhyamhmdy9wZ3duemIya1F4NE40NFFpK1M3eFQ3cGN0WUc1NkNkRjY4VnRBSXlBQ2x3Mi94MFhycVd3UzIrVUVDYTBHL2lNWnRPVTZHNUJWMkVSSnlMVzZkS09Vais0NHlYWUM2T1lZVFRsUDFGeXA5OElWOHlFYjRXbDZuSHNCd0ZVcHZJanlrWStKRjcyMHpqL0VMTnJMQzBnYW1mTHlORFZrQUliaE9hcCthUVZ2c2tlWFVBV213WDNoM1pTWlhoOGo5OUtPMFJJbG1kMnhvL0FwcEpWTnovUkNlZ1cxV2hpdGk2V0FOUWdENXhPNjkzQjM0Y1JBMUxMSzM1YStsOWx6TSszbisrVm9xd3FjTlh5WE12alBOYk9yK3d4QXUxZDFERUxvc3BaeWNuNXovNG5obXlINHk5TDh5cU5BUWUvZUw1d0ZGT3BrZ2ltRTNGeE84Z2UwdFloRGNTWXVLYVM4MzQvZzRXSWliem5wckhsR25ONmZwNVFaV0x5TVdXbEV4TSsrWmdzTjV6dE5DWWZmbVBvUnl6akJ0cGFPN1JadHdOdHhkQUN6QzBIcCtiMzZRV3RrZ3A3VmxyQmVFZ1JpeU5DY3cxR2IyVVFrZXZkZmtSYnRsTUphOFRlWHNuaThmQ2RvbDZLS2Z4R2Q3UUMvUS9MVlAxZkhFSGcrMjlyZ3g5VU5XbkRqWDBvajRmNm1JTUpKMjBmQnBtc0hwTEZ2cWhDUGdkV21tR01YZVVXcFFHNWd2OTd5OXdIVEtHdTJGWU11NlV4MUp1dHVCNlpuVG5SZCtwcDArdDJaaEVGNzlyWnNsMUlSYmswMjNaK0UwbDRvZlFMQ3dyY3lnYUlsdXpWcHRwbVpVaVl4a2lnV0V1YlN5dHdFOVlSL0JRQ1V5OGlNMEgxcDc1Z25MMzRlL016WXpwSFl5OUlWVE5RN1NETmNRcUcxM3dKSW5rblVFZ3pEUTVFUFBNL3VjNHRVanA1TkZnWTNTRVRleHYzdzU2TmJFQ082K0R0RUxKZ3IwVWtWSW8vNmMrNWs4VHN3MmdMVDViZUZsalNPMkJMUm9BeVk4UWx0a3VrYWVoT3VwdjRtN0NBZWc3VTkzR1RKMjRMSTRReVBwNHBHbmxUaG5NdjRSd2U4WmJxR01vRDZsM1U4cjZ6QTF5c25WN1RHTzFCZUFWdEFocW1CWldUd0RQTGlGUlpEbHFacCs0N2FyU2YrYVBtWXN0Qyt0aW9Lbndna0pnbHgxRithYUNhZ1VsakZVcnBocUFGYURlbjh2MWlFaWoycXE3ZlBYTVloSkNVUlVsOGRwVVY2UE1aNW15K2Q1citPcWM2Y0RXZ0N0UnBQOUgzSVFPMzcwMVJ4MnM4OVBKNnlaQUxIY25yalN6S083WnpzN3ZUWE5hTlMyN0ZYbFJJMlV5UUl1UnBqbE1KTFppSTh0aWhZWXE2Y01XemgwR1pJSTFuOEpLeEZsU1pMYzRJUm4zaGFWUkgyTFVJS3YwejRETlM0d3ZaNDU3R05NditHbC9FSFBFYXpkMnR6NXFqZlRxOHN4TDlYZWZHREpJVUFGTHkyczl4cVBBTkxmdVhhTUJ4cms4blBTYXZGNUovZ0dsQ2tkUTd6cHdMYituWkVWY3dNZnBvU1BQSFBTc3hkTEN0dmErMTA3TUVNMjA3NW5EdXB6cW95dCtpYlZrR1drSklSYzM2MWFzbEFPdVk1WHdTQ0VWcHI0TnZFWGNrMmptdW9OUnk1RUVGelQ4Z3lmNkhad2hROVJzUFBLRnRCTCtJM3M4UmNKaGhXMEtnT0w2OGEzTkV2cmUyMFNGUmR1Z3ZYLzRQNHJZdlVuTkYza3FIR2RBZEV3cEVFUEU0S1NoajBQdXU4OFJhdDhhZVdzUmJ1NHNEeUZ2VkJhZ0ErZ05Dczl6R25oYzhTVy9Ub0FxU2hXcEdGVkVXUkxYTnM2ZURJcU9QUGR1d1RBTXNxT2FGYndpYmpJRGozSTlEUFdjdjhhWVB2TS9iWDl4dFo1MXoyMjdZUThFaTR2UDlHcWhhbi9sQkxNTEx6d1Q3WDkzTjVjV2t0VWhuZEI1TlI0K1JxUzZoZzhPM0d6b2NPMnNmcFk3akNQRk0vR3U2VWJUckxnUFJvMDVGc1RVZWNRL0hHaEVCWEt1aThqdFhnV3Z2c1I2aEhxQ2JkNlNVK0VueEJBVWxMS1dBc2RTZ0E2TzJPWFhPeCtLTng0dWNCMHQrMGRVeXN4Mys4YjJtTGZMQ2lYSHRINEcweDQ0V0tHOXRCWk0zcE5mNDZCeW9rNVhzVjYybkhYZkY3UGczUHoxZHN2dGgxK0N1UGNENVFZeGFsTTY0VlNEUi96c0I5YUhKdCtqdmR3QmlaRkJVOWZrWG9CbllkcGV1NVhob2NoSWxzNWhYWEk4VzBGdVhqb1FieEFGR3RuWExqQ1B4S090dXI2Nm9oOWFFeXY5NGtjUkhKS2IyWVdza1JPSVJvZmJFYi9YblNaNGxUalZ1TXNSMks3M0tjd0g0L256dytlZ0lnPT01MWRhcTExZ3NNc0QzVFp0IiwiZXhwIjoxNjQ5Mjk5MjMzLCJzaGFyZF9pZCI6MjU5MTg5MzU5LCJwZCI6MH0.XvENhTEmiL-pNgs-IiCKt9u0ZwPs8TCJyWof4kHzyno";

            HttpRequest r = new HttpRequest();
            r.ConnectTimeout = 5000;

            string url = "http://2captcha.com/in.php?key=449cfe7c13d2fe0f98b01392f63759e4&method=hcaptcha&sitekey=3e48b1d1-44bf-4bc4-a597-e76af6f3a260&pageurl=https://hhsen.ml/Kasper/tell.html";
            string res = r.Get(url).ToString().Split('|')[1];
            string output = "CAPCHA_NOT_READY";

            while(output == "CAPCHA_NOT_READY" && res != "")
            {
                Thread.Sleep(5000);
                url = $"https://2captcha.com/res.php?key=449cfe7c13d2fe0f98b01392f63759e4&action=get&id={res}";
                output = r.Get(url).ToString();
            }
            Console.WriteLine(output);

            if (output.Contains("|"))
             return output.Split('|')[1];


            return "";
        }

        static void Register()
        {
            // https://api.tellonym.me/accounts/register
            // {"country":"US","deviceLanguage":2,"deviceName":"samsung SM-N975F Android 7.1.2","deviceType":"android","deviceUid":"512f42b99ea4b5cb","lang":"en","activeExperimentId":"0","hCaptcha":"P0_eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwYXNza2V5IjoiMmk2WXdqTDhIazliTDBsVTVrQ1orMlFUMS9reWZMWjdTYVduRzFuTDBCVE1ISVVmeHRtbEY4dG1hRkxIZ1lKQjNxTnZkUHB2TUtUSzdLbUdORzFEV29xS3IzM09JQVhicTUxcDlRdENudSswQjZVY1pVRWdLamRkZmM5b2lrWDVvRFBTNXJhbFJnQ0Jhb1hPTy94N3lTNjVXbGY5WWdwQ1hyb0xDNFM1Ty9xTVMyVHNiN2N1eldGZ0VaUU9GNU9YV1QxMUtraTE0Mm9zaEpjTXQ3NWJTbWluZFhuOGFQNWVyZ0VuNk5sYTVsb1RwejVidnNnTDlBZjNzWUFvWFI4bytIZUZsbU8xam9tMEtxV2V4TXVyM0F1RFNGbDRET1Y3SktjTDlrL3VmYm1lWVcrTHIxU09JUlJrN0Q4TWorSVVVWk1SRVNENFN1US9jdzBna0NucWxFSXVNRVV3VGp2VU84NGFEK2FuQk9Za2dUZ2UrOWxKZEkrRm5EeEltdklobUhVb0U1Z0VwVDZsblhHd083SkhMS2ZIaFdxZTI0cWpCcHV6QnZlMG1jckErYk9lR1R6QTZSOTZvZlVOQjVWWlFiT3liTTZJbGRmSFRvWW03V2V0cmtJS01HR0NDczZsUTY0U1VOOGc5WUxGZTlPRFB2S3N3S2hobDg0QVlsdW04Z1doeDNqYk1VK3RETVo5NVluSTdHWTBTZng2NWtkWU11QjhsOTF4NUtHVTBMbFBKc3lEbDgyVHpDbWMwakdsU09yZUJiUFFGN1BxZDM0dUJkL1lCaDFQaWwySkRQUGxjQTZpOUIvVEU0K21QdkltSVJGUzR0eHh5WTJYWCtEdVhYUmtlaXV0U3lLS3Z2dndIMkVpKzhHU0wzN1hadmdITDJkSVNxVzN6aTRDUzRuME5TejlWN2srMkJ0Tk9sRmFwWWlxMUt4QXFXQ1FmYkJHdHFGaGxrM09TbWY1dkR1aEtsS2FSSWNjMkMxMkNwdHRFQmE4M0crUG5tVjZveXVjT1l1dWFuTlppOXFLNktDSnhXa1NhVkRSdGo2MDdrNmQ1Zk5UUVNjWVAyd1RUOHoyYkRlSjU3MkVHdlE1TmdzQVgwZXZoV2lkQmY2ZTQzQUlvNXNJd0piRGcwRHRMZWtQeGlxQTE2YWltWGRJNVFZZmJHakFJN2dIbVNua3lHQlluTWgra2dtYmNTWVk1NzFXOGJEVS85aWYzTGZ3NWFPaTFEeG5yQ0djak1RbGhpdUM1SkxPOWk0R2pDZ1dOajJjTHowMjgxbDZXZ3NUbVJ0RVFRdWhuYm5ZT2l4M1NxZGlET0pQa2gyVytRNGNDa0RqZVRFclFmamRPczBYVTZTcmdVYzlENDF0WnlCMUhtdE9xbUNnTmlqZjZIektVL1paYXY1c3QvbGNna0VVV3k5WWhIOFdMQlN2K2dSUi85MUVsZmY3TDVMWjBOZk91Z0lKT0x1NEtTMnJTaWYyMncrVk1KWlAzTmJvZzB4MG5LZ0w5ZzgyQzc2eFVmY2MzTFZ5STY2amZMUmNOdGgzeTlZZ2dDRG81aVlCTThSdlgrc3FPM3FGUmVZdy9YQUJSU1BmdWNDTDlNaWppSVN6ajNkeVJRNGtZcHJ5SEc0dUFGeXJiQVRLazY5c2RWL0hiTllITjJJMWF0WjQ1WWxTbzFFeHJZTms0QjZ6M3ZBamNwWFVlZ3l5RjM4bHdFZ0xXRGlyUDZYRVgxaEZjUEhyekxDczRRcW5PaHVUSy9OQ0VyQkx5UmJtcVFMb1RxNG5FVm00QUYzRnlHWWY1TWhkZlVIdHd4VHBWS0ozYlo4aWhxSUhxM2RRSkk1Tk9aMG9zSDVtUng3YzNhMnlrQmZWVDFSVFRwNVpDdmMvQzUxTXlkMkR1L3B2bnltT0xKMWF5RmEzeVI5SU8zdCt4YWNOMzdGY3k0K1ZxdDdlM3M3aHhKZ3VPaytZZTJTckRGdTVqWjc2b0FqZitsWjhzSVpOSFBvK3NuaGkrRkVlS3k5OUNJUExrVkR2QWRRSU1qa2sxQ1FpRW5hSGhuYWF6WmV1ZlZPNFBNc3JiK21SZnR1SDFMMi8yaGMyZDZkTmR2cmRMLzcyKzB4MG1DcjRtNDBDSFY2aWIwZTRGbzc2ZlhDcVlLL0R2UkcyOG0xVVA3TU9ZUXpDcithVGFnRWllaFBsZGs0Wm5LTVV3NUt0bzRpZU9meTJiaUNESk9leklKSkVvWmVYbjBDclROZVBpQ3ViM2VDbEdNckdJakpvSVhCNGhneTBTSlFYU25OR2xpZUJZWnBSSWRiYnp5ZDdNc0ZkZUlkOTh6eDlNR281VHpESTJHODBsOTRJbTRNRDBQZFhXSThDVXlnY0ZaRkg3QUtsYXgxN3FJdkZDTW42dzNhbXUrTzNORXhRNjhKMEpUclduSGJpY01Ja2F1NjlaYWVnUXBIYWNPdEUxYmQ5RjNuOEhMN3NrWEQxbnRWS0wxR084dE5JTDQ3WnRzZ3B0UkVmOVovZStobzZ0bzhwdlpqbHJEeFFzRE5uQXZlM0R4S1ZZdnVkd2tLWDVDbkhLSWxLS0JsRmhLRkVqa2JIeTVsYm1lTU41R0ZOcWFCSmtSdmI1L3YyQUJYYkJVSW83RWc3VDZzRWhDam5DcFJpaFFyM2RaSHFIMnpYSWFTczVSMVIxak5keWI3MjYvWnR4RXZXeFpKVTlsSUtEdlc2bWxzZmZhS0RzUThBd25JZjZFdlBkV3AveU8vZlFRYVpUcGpPc2dGbUx0b213UllDenR6QTVDTUJIZWNraWd2UmJrL0lMaCtzMW51NmZZOFdzY3c5NkVzdWRrdXJ2cXJLOGZnZkE1ajMwNmlodWRMeG4zV3NvUHArNHFPbDZFSG9ONC9wQUp1WTR5ZVZPeTUyL29JK3I2UG5UdUNzTkF5VVNHK252TDVrOHVFNFFsbEw0dDVUa2lxeERxbkhHRlZod2kxYklsQkhhNkxCN1JkSTNWWGxDOFF1VzFiOVhvOVpna1AxRGVaWHJySzQ2N2ovOU5BaSs3STh6S1psQ25PckFrNjRleTRkSS9VcS9ZVnJiMXY3K1pnL0ZNbDhnQmNtam5XL3VYam5MelVpRkQvU3VyZ3gyM3dCaXR3TGRtbEtoZGJyZTl5NWZkMW5lSVkvbEIwNEtlMEpFYnM0SmEwbXczOUt1UGFMYWt1Nm4vZ0NKMGNCTGczZGRUYWhBY0N2bENnUTRSVi8yU3d5NGNETzRSc21YeTgzYzJHWVg5VDUwWmd6WnRBOHV2YnVrLytRb09iSjgxWlc0cDBRNXcwTldPUXVhbmpDK0piUE4xSzBqaWRCS0ZDMlRreTU4U29MeUtXVVF1RnVTNTJNREhZYXAzamNxN0ZZcmYvTFNveGhTZ0hjL043NkFZUG5BNFNCSHd4QzJJLzIrZjQvRVZBYlh5aDJLWnByOE1xYys5LzRweit5U0FvOHp5elFCWEFFd1dsaXNqQWNXZ2VEVlJHWENKcU9KbnFaSEpKdlFxaThDODhQRWZ4T1VRTTJ4bHF0MUVrYmxvN3JqRWhQeUVkdmM1RElhSVVRYW9mTHNWK3BjUThQOXhpWkg5c2llU0xmazRQK3ArdHcydE9WNlZoOTVnZHdMVUR0RkhtSE40cVcySFpWdjkweFlrdWtQUEdUbWZ3cURpRWpWZ0orY0FMUnQzQ1F3TTM0Qm5xengwUkVEcVZBM1QrNVliQjcxczFmWC9renZ3MEtrN3lEZzRHRmVlblhMaVVCbmI4NXY4MEJuNTBpWmk0YnBReGRHQktUSEN6b25ZaGxpZ1V1Q1VnejhzY25zSWVIU1d6aFhHRW5KSUwrZlVoaGdyRldnYndiTkxleDZZUFR2MGdNb3d1WEc0S2xmUDNBL2k2dzltY1hmeGRPRXhqQ1hZOE5zNFhlTDRxNlV1c0NaUlh3cVJIQisyMm55bE9VYVhNOWZRcTJvY0xld3RqZjZFbGZHN2NBUG1HeEJBd0tVNlV0S1hqUzExNWVITU5Jc2M2MkJXQ0R2eE41UHRWcDVNandkd3VpRE55Rk5wV29EK0htellkaC9lS25CdGJPZTY4QXVCb2E5ZWpubTNGemtaSys3YlpZOHNjTk5NVGJRaGRLcTkxYWoydDBFSVVtcTRxVms3R1lWU2xmYk5yd0h1bHNjOVlpM0x3Z3c5SFpZUFJTUHM3LzhYZ2s0aXZHaVNxZW5oS3NOZzBFSU9DaDJxYUZyMFNvQXJVZ2hXVFUrREtITXdQS1ZmZGtQSFN4azJxblJ0R3VlazBDK0U1ZTFyWm5lU2ZuRisyU29HcDFYVmk1TmtNVGFIYkRqczRDM0lKejZicFVlWEEwUGptK0JyVE5oTnh1OHMyTGZIMXVzcFV4dU5MZ1VPZkdDY3k1dnpmZ29paUVFSjVsUWxaMTBPc3dWekZ1UzBXS0JKR1RnbEorK3Y5THdEUzZoUTVrcE9zTlRLaGVlQWZwSXowaEd4MFhDMVlxSWwrd0hKeGdaQTRZTzFQbTRMQmxzM0ZKZlowcTBIODhWWEJYNDdiNm5hamtSSjZFTUFjSHpISFFGbURDa1JGNGtMbERXYk00MHkyM05BaS92V0ozb1RDY0QyTGsxUm42azJwYTN1dUNoV1F1ZlU4OVhDUVQrQ2h1MmJHL1F3S043S3NMeTlKQzllY1lyU3RUemQxSkg2M3ZoekU3R1ZPWDYxM2tTaEU1alM2VGZ5SUdpazBGM1drVDdOdW9NU3g1RmhwQllFRXd5eHJQbTY3TE1UU0wrOVdLNmpVNnlFOFhZYzlPazFLRitnPT1wemhjbGFjcUhhQnltNUlJIiwiZXhwIjoxNjQ5MjgzOTQ2LCJzaGFyZF9pZCI6MjU5MTg5MzU5LCJwZCI6MH0.NG2i4d0wClyMvwAfx-H3DTHg9fV5GAnpQzaT4MDUiEM","email":"ea3a6b9905@catdogmail.live","idfa":"f074d0e2-95a4-4557-b5cb-65db343c870f","password":"ea3a6b9905@catdogmail.live","username":"asdasdasd","limit":16}

            string c = GetCaptcha();

           
            string dv = Guid.NewGuid().ToString().Replace("-", "").Substring(0, "c0fa0dffdeb3d0e0".Length);
            string postdata = $"{{\"country\":\"US\",\"deviceLanguage\":2,\"deviceName\":\"samsung SM-N975F Android 7.1.2\",\"deviceType\":\"android\",\"deviceUid\":\"{dv}\",\"lang\":\"en\",\"activeExperimentId\":\"0\",\"hCaptcha\":\"{c}\",\"email\":\"{dv}@catdogmail.live\",\"idfa\":\"f074d0e2-95a4-4557-b5cb-{dv}\",\"password\":\"{dv}@catdogmail.live\",\"username\":\"{dv}\",\"limit\":16}}";

            string res =  PostReq("https://api.tellonym.me/accounts/register", postdata);
            string token = Regex.Match(res, "accessToken\":\"(.*?)\"").Groups[1].Value;

            Console.WriteLine(token);
            if(token != "")
                File.AppendAllText("token.txt", token + "\r\n");

        }


        static void checkproxy()
        {
            string[] prox = File.ReadAllLines("proxy.txt");
            foreach(string p in prox)
            {
                //Console.WriteLine("Check: " + p);

                try
                {

                    HttpRequest r = new HttpRequest();
                    r.Proxy = HttpProxyClient.Parse(p);
                    r.ConnectTimeout = 5000;                
                    r.IgnoreProtocolErrors = true;
                    r.AddHeader("cookie","cf_clearance=qtFK8ZFNl1wxDm6xvcXyoC.K.89sMAptE4BaVlM4Kf4-1649355597-0-150; _ga=GA1.2.1527684714.1649355600; _gid=GA1.2.1999949673.1649355600; __cf_bm=77b4SVuCUGExRfwJWvEunDd6tjOpxJ04RXyCg6n7yEs-1649355601-0-AQXXdgScDTivhXAZQ6d4+Ynjqcz9hLggsj/uD9YnODQzF8nNgnfpIp3Juh3IbViyLhw2k+/XZXccPuupUtYlRPj/GqOxc+VXWrx6RcWi1XT8TvuIOtIYp2wCrpv/nTArMQ==; __rtgt_sid=l1pbqn2473hba2");
                    r.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.75 Safari/537.36";


                    string res = r.Get("https://tellonym.me/").ToString();

                    if (res.Contains("code") || res.Contains("tellonym"))
                    {
                        Console.WriteLine("Good: "+p);
                        proxy.Add(p);

                    }

                }
                catch { }
            }

        }
        static bool SendMsg(string id)
        {

            // {"senderStatus":0,"previousRouteName":"ScreenResult","contentType":"CUSTOM","tell":"فديتتتك","userId":83227116,"limit":16}
            string msg = File.ReadAllText("msg.txt");
            msg += "\\r\\n\\r\\n" + randomString(2);

            //if (r.Next(4,10) % 2 == 0)
            //{
                string PostDataa = $"{{\"senderStatus\":0,\"previousRouteName\":\"ScreenResult\",\"contentType\":\"CUSTOM\",\"tell\":\"{msg}\",\"userId\":{id},\"limit\":{r.Next(10,99)}}}";
                string url = "https://api.tellonym.me/tells/create";

                string prox = "";
                try { prox = proxy[r.Next(0, proxy.Count - 1)]; } catch { }
            if (PostReq(url, PostDataa, prox) == "OK") Console.WriteLine("Send: " + id + "\t\t" + prox);
            else { Console.WriteLine("Bad: " + id + "\t\t" + prox); Thread.Sleep(60000); }
                return true;

            //}
            //else
            //{
            //    string PostData = $"{{\"isInstagramInAppBrowser\":false,\"isSenderRevealed\":false,\"tell\":\"{msg}\",\"userId\":{id},\"limit\":13}}";
            //    string url = "https://api.tellonym.me/tells/create";

            //    string prox = "";
            //    try { prox = proxy[r.Next(0, proxy.Count - 1)]; } catch { }

            //    HttpRequest re = new HttpRequest();
            //    if (prox != "")
            //        re.Proxy = ProxyClient.Parse(prox);
            //    re.ConnectTimeout = 5000;
            //    re.IgnoreProtocolErrors = true;
            //    re.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.60 Mobile Safari/537.36 Edg/100.0.1185.29";

            //    string Result = re.Post("https://api.tellonym.me/tells/new", PostData, "application/json;charset=utf-8").ToString();

            //    return true;
            //}


            // {"isInstagramInAppBrowser":false,"isSenderRevealed":false,"tell":"asdasdsad","userId":83227116,"limit":13}
        }
        static void GetIdes()
        {
            // https://api.tellonym.me/suggestions/people?oldestId=&adExpId=8&limit=50&pos=30
            string lastid = "null",
            limit = "50";
            int pos = 0;

            string ak = File.ReadAllText("id.txt");

            while (lastid != "")
            {
                if (lastid == "null") lastid = "";
                string url = $"https://api.tellonym.me/suggestions/people?oldestId={lastid}&adExpId=8&limit={limit}&pos={pos}";

                string res = GetRequest(url);
                MatchCollection matches = Regex.Matches(res, "\"userId\":(\\d+),");

                foreach (Match m in matches)
                {
                    if (!vs.Contains(m.Groups[1].Value) && !ak.Contains(m.Groups[1].Value))
                    {
                        vs.Add(m.Groups[1].Value);
                        SendMsg(m.Groups[1].Value);
                        File.AppendAllText("id.txt", m.Groups[1].Value + "\r\n");
                        //Thread.Sleep(10000);
                    }
                }


                matches = Regex.Matches(res, "id\":\"(.*?)\"");
                try
                {
                    lastid = matches[matches.Count - 1].Groups[1].Value;

                }
                catch { lastid = ""; }

                Thread.Sleep(3000);
            }

            Console.WriteLine(vs.Count);
            //return res;
        }

        static string PostReq(string url, string PostData, string proxy = "")
        {

            try
            {
                // POST






                HttpRequest rr = new HttpRequest();
                if (proxy != "")
                {
                    //HttpRequest r = new HttpRequest();
                    rr.Proxy = HttpProxyClient.Parse(proxy);
                    rr.ConnectTimeout = 5000;
                    rr.IgnoreProtocolErrors = true;

                    //rr.Proxy = ProxyClient.Parse(proxy);
                    //rr.ConnectTimeout = 5000;

                    //rr.Proxy = HttpProxyClient.Parse(proxy);

                }
                rr.ConnectTimeout = 5000;
                rr.IgnoreProtocolErrors = true;
                rr.AddHeader("tellonym-client",$"android:3.{r.Next(1, 99)}.6:{r.Next(1, 255)}:7:5{r.Next(1, 255)}42b99ea4b5cb");
                rr.AddHeader("authorization", "Bearer " + RandomToken());
                rr.AddHeader("X-Forwarded-For", $"{r.Next(1, 255)}.{r.Next(0, 255)}.{r.Next(0, 255)}.{r.Next(0, 255)}");
                rr.AddHeader("X-Real-Ip", $"{r.Next(1, 255)}.{r.Next(0, 255)}.{r.Next(0, 255)}.{r.Next(0, 255)}");

                //rr.AddHeader("X-Originating-IP", $"127.0.0.1");
                //rr.AddHeader("Forwarded-For", $"127.0.0.1");
                //rr.AddHeader("X-Remote-IP", $"127.0.0.1");
                //rr.AddHeader("X-Remote-Addr", $"127.0.0.1");
                //rr.AddHeader("X-ProxyUser-Ip", $"127.0.0.1");
                //rr.AddHeader("Client-IP", $"127.0.0.1");
                //rr.AddHeader("True-Client-IP", $"127.0.0.1");
                //rr.AddHeader("Cluster-Client-IP", $"127.0.0.1");
                //rr.AddHeader("X-ProxyUser-Ip", $"127.0.0.1");
                //rr.AddHeader("", $"127.0.0.1");

                /*
X-Originating-IP: 127.0.0.1
X-Forwarded-For: 127.0.0.1
X-Forwarded: 127.0.0.1
Forwarded-For: 127.0.0.1
X-Remote-IP: 127.0.0.1
X-Remote-Addr: 127.0.0.1
X-ProxyUser-Ip: 127.0.0.1
X-Original-URL: 127.0.0.1
Client-IP: 127.0.0.1
True-Client-IP: 127.0.0.1
Cluster-Client-IP: 127.0.0.1
X-ProxyUser-Ip: 127.0.0.1                 */
                rr.UserAgent ="okhttp/4.9.1";

                string Result = rr.Post(url, PostData, "application/json;charset=utf-8").StatusCode.ToString();
                return Result;

                //HttpWebRequest httprequest = (HttpWebRequest)HttpWebRequest.Create(url);
                //httprequest.Method = "POST";
                //httprequest.ContentType = "application/json;charset=utf-8";
                //httprequest.KeepAlive = false;
                //httprequest.AutomaticDecompression = DecompressionMethods.GZip;
                //httprequest.Timeout = 5000;

                //if (proxy != "") httprequest.Proxy = new WebProxy(proxy);


                ////httprequest.Proxy = 
                //httprequest.Headers.Add("tellonym-client: android:3.20.6:805685:7:512f42b99ea4b5cb");
                //httprequest.Headers.Add("authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ODMyNjM5NDcsImlhdCI6MTY0OTI4MzgyNn0.ZUQOB6bgJRtItO-NU_6WtN2HXLb97uHNHgHcd5GTHHY");

                //httprequest.UserAgent = "okhttp/4.9.1";

                //ServicePointManager.UseNagleAlgorithm = false;
                //ServicePointManager.Expect100Continue = false;
                //ServicePointManager.CheckCertificateRevocationList = false;
                //ServicePointManager.DefaultConnectionLimit = ServicePointManager.DefaultPersistentConnectionLimit;


                //byte[] bytedate = Encoding.UTF8.GetBytes(PostData);
                //httprequest.ContentLength = bytedate.Length;
                //Stream PostStream = httprequest.GetRequestStream();
                //PostStream.Write(bytedate, 0, bytedate.Length);
                //PostStream.Close();

                //HttpWebResponse Response;
                //string Result = "";
                //byte[] data = { 0, 0 };

                //try 
                //{
                //    Response = (HttpWebResponse)httprequest.GetResponse();
                //    Result = new StreamReader(Response.GetResponseStream()).ReadToEnd();
                //    return Result;
                //}
                //catch (WebException E) 
                //{ }
            }
            catch { }
            return "";
        }

        static string GetRequest(string url, string proxy = "")
        {


            HttpRequest r = new HttpRequest();
            if (proxy != "")
                r.Proxy = HttpProxyClient.Parse(proxy);
            r.ConnectTimeout = 5000;
            r.IgnoreProtocolErrors = true;
            r.AddHeader("tellonym-client", "android:3.20.6:805685:7:512f42b99ea4b5cb");
            r.AddHeader("authorization", "Bearer " + RandomToken());
            r.UserAgent = "okhttp/4.9.1";

            string Result = r.Get(url).ToString();
            return Result;

            //try
            //{

            //    // POST
            //    HttpWebRequest httprequest = (HttpWebRequest)HttpWebRequest.Create(url);
            //    httprequest.Method = "GET";
            //    httprequest.KeepAlive = true;
            //    httprequest.Timeout = 5000;
            //    httprequest.ContinueTimeout = 5000;
            //    httprequest.ReadWriteTimeout = 5000;

            //    httprequest.Headers.Add("tellonym-client: android:3.20.6:805685:7:512f42b99ea4b5cb");
            //    httprequest.Headers.Add("authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ODMyNjM5NDcsImlhdCI6MTY0OTI4MzgyNn0.ZUQOB6bgJRtItO-NU_6WtN2HXLb97uHNHgHcd5GTHHY");
            //    httprequest.UserAgent = "okhttp/4.9.1";
            //    //if (proxy != "")
            //    //{
            //    //    WebProxy a = new WebProxy(proxy);

            //    //}
            //        //httprequest.Proxy = ;


            //    ServicePointManager.UseNagleAlgorithm = false;
            //    ServicePointManager.Expect100Continue = false;
            //    ServicePointManager.CheckCertificateRevocationList = false;
            //    ServicePointManager.DefaultConnectionLimit = ServicePointManager.DefaultPersistentConnectionLimit;

            //    HttpWebResponse Response;
            //    string Result = "";
            //    byte[] data = { 0, 0 };

            //    try
            //    {
            //        Response = (HttpWebResponse)httprequest.GetResponse();

            //        Result = new StreamReader(Response.GetResponseStream()).ReadToEnd();
            //        return Result;
            //    }
            //    catch (WebException E)
            //    {
            //        try {
            //            Response = (HttpWebResponse)E.Response;
            //            if(Response != null)
            //            Result = new StreamReader(Response.GetResponseStream()).ReadToEnd();

            //        }
            //        catch { }


            //    }
            //}
            //catch { }
            return "";

        }


        // https://api.tellonym.me/tells/create
    }
}
