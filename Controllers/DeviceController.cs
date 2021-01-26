using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetworkMonitor.Data;
using NetworkMonitor.Models;
using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace NetworkMonitor.Controllers
{
    public class DeviceController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DeviceController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            // lay cac record trong device if email trong devices table == email session 
            
            IEnumerable<Devices> objList = _db.Devices;
            return View(objList);
        }

        // GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        // POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Devices obj)
        {
            if (ModelState.IsValid)
            {
                _db.Devices.Add(obj);
                _db.SaveChanges();
                ViewBag.message = "Added device successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.message = "Add device fail";
            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Devices.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            IEnumerable<Category> categoryList = _db.Categories;
            ViewBag.Result = categoryList.ToArray();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Devices obj)
        {
            if (ModelState.IsValid)
            {
                _db.Devices.Update(obj);
                _db.SaveChanges();
                ViewBag.message = "Updated device successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.message = "Update device fail";
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Devices.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            IEnumerable<Category> categoryList = _db.Categories;
            ViewBag.Result = categoryList.ToArray();
            return View();
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int? id)
        {
            var obj = _db.Devices.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Devices.Remove(obj);
            _db.SaveChanges();
            ViewBag.message = "Deleted device successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult Scan(int? id, string comm, string ipAdd, string deviceCategory)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Devices.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            IEnumerable<Devices> deviceList = _db.Devices;
            foreach (var device in deviceList)
            {
                if (device.Id == id)
                {
                    comm = device.Community;
                    ipAdd = device.Ip_Address;
                    deviceCategory = device.Category;
                }
            }
            Ping ping;
            PingReply reply;
            IPAddress pingIpAddress;

            string ipPing = ipAdd;
            pingIpAddress = IPAddress.Parse(ipPing);
            ping = new Ping();
            reply = ping.Send(ipPing, 1000);

            if (reply.Status != IPStatus.Success)
            {
                ViewBag.Message = "Can not connect to the host: " + ipAdd;
                SendMail();
            }
            else
            {
                OctetString community = new OctetString(comm);
                // define agent parameters class
                AgentParameters param = new AgentParameters(community);

                // Set SNMP version to 1 or 2
                param.Version = SnmpVersion.Ver1;

                // Construct the agent address object
                // IP Address is easy to use here because it will try to resolve contructor 
                // parameter if it doesn't parse to an IP Address
                IPAddress ipAddress = IPAddress.Parse(ipAdd);
                IpAddress agent = new IpAddress(ipAdd);

                // construct target
                UdpTarget target = new UdpTarget((IPAddress)agent, 161, 2000, 1);

                // pdu class use for all requests
                Pdu pdu = new Pdu(PduType.Get);

                IEnumerable<OIDs> oidList = _db.OIDs;

                int numberOfOid = 0;
                foreach (var oid in oidList)
                {
                    if (oid.Category == deviceCategory)
                    {
                        pdu.VbList.Add(oid.OID);
                        numberOfOid++;
                    }
                }

                List<Result> discoverResult = new List<Result>();

                SnmpV1Packet result = (SnmpV1Packet)target.Request(pdu, param);

                // If result is null then agent didn't reply or we couldn't par se the reply.
                if (result != null)
                {
                    // ErrorStatus other then 0 is an error returned by
                    // the Agent - see SnmpConstants for error definitions
                    if (result.Pdu.ErrorStatus != 0)
                    {
                        // agent reported an error with the request
                        string errStatus = Convert.ToString(result.Pdu.ErrorStatus);
                        string errIndex = Convert.ToString(result.Pdu.ErrorIndex);
                        ViewBag.message = "Failed! " + errIndex + errStatus;
                    }
                    else
                    {
                        for (int i = 0; i < numberOfOid; i++)
                        {
                            string sysOid = result.Pdu.VbList[i].Oid.ToString();
                            string sysName = SnmpConstants.GetTypeName(result.Pdu.VbList[i].Value.Type);
                            string value = result.Pdu.VbList[i].Value.ToString();

                            string[] row = { sysOid, sysName, value };
                            discoverResult.Add(new Result
                            {
                                Oid = sysOid.ToString(),
                                OidName = sysName.ToString(),
                                Value = value.ToString(),
                            });
                        }
                        ViewBag.Result = discoverResult.ToArray();
                    }
                }
                else
                {
                    ViewBag.message = "No response received from SNMP agent.";
                }
            }
            return View();
        }

        private void SendMail()
        {
            try
            {
                string fromAddress = "thuynguyen080700@gmail.com";
                string toEmail = HttpContext.Session.GetString("Email");
                var toAddress = new MailAddress(toEmail);
                const string fromPassword = "TN080700";
                string subject = "Notification!";
                string body = "Your system have some problem, check it!";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromAddress);
                    mail.To.Add(toAddress);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                        ViewBag.message = "Can not connect to your device, we have sent you an email, please check your inbox to learn more. Thank you!";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = "Can not send mail, please check you connection!";
            }

        }
    }
}
