using LIS.DtoModel;
using LIS.DtoModel.Models;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace LIS.Com.Businesslogic
{
    public class DxC700SerialCommand : SerialCommand
    {
        private readonly DxC700Configuration fieldConfiguration;

        public DxC700SerialCommand(PortSettings settings)
            : base(settings)
        {
            var path = $"{Environment.CurrentDirectory}\\Data\\DxC700Configuration.json";
            var config = File.ReadAllText(path);
            fieldConfiguration = JsonConvert.DeserializeObject<DxC700Configuration>(config);
        }
        public override async Task DataReceived()
        {
            //Logger.Logger.LogInstance.LogDebug("DXC700 DataReceived method started");
            var input = port.ReadExisting();
            Logger.Logger.LogInstance.LogDebug("DXC700 DataReceived input '{0}'", input);

            // TODO Future Change this to StringBuilder
            sInputMsg = string.Format("{0}{1}", sInputMsg, input);

            if (input.EndsWith(((char)3).ToString())) // Ending of string <ETX>
            {
                Logger.Logger.LogInstance.LogInfo("Read: '{0}'", sInputMsg);
                string commandText = sInputMsg.TrimStart((char)2).TrimEnd((char)3);
                await Identify(commandText);
                sInputMsg = string.Empty; // Reseting global value
            }

            //if (input.StartsWith(((char)2).ToString())) // Beginning of string <STX>
            //{
            //    sInputMsg = string.Empty; // Reseting global value
            //    sInputMsg = string.Format("{0}{1}", sInputMsg, input);
            //}
            //Logger.Logger.LogInstance.LogDebug("DXC700 DataReceived method end");
        }

        public override async Task SendOrderData(string sampleId, string sampleNo)
        {
            try
            {
                Logger.Logger.LogInstance.LogDebug("DXC700 SendOrderData method started for sampleId: '{0}' SampleNo: '{1}'", sampleId, sampleNo);

                string patientGender = string.Empty;
                string patientName = string.Empty;
                string patientId = string.Empty;
                string patientAgeYear = string.Empty;
                string patientAgeMonth = string.Empty;
                //string trailerSegment = $"{Strings.Chr(2)}SE{Strings.Chr(3)}";
                IEnumerable<TestRequestDetail> testlist = await LisContext.LisDOM.GetTestRequestDetails(sampleId);

                List<Dxc700Tests> testlst = new List<Dxc700Tests>();
                string specimen = "";
                if (testlist.Count() > 0)
                {
                    var firstTest = testlist.First();

                    specimen = GetSpecimen(firstTest.SpecimenName);
                    patientName = firstTest.Patient?.Name;
                    patientId = firstTest.Patient.HisPatientId.ToString();
                    patientGender = firstTest.Patient?.Gender?.Substring(0, 1);
                    var age = firstTest.Patient?.Age.ToString();
                    if (!string.IsNullOrEmpty(age))
                    {
                        var ageSplit = age.Split('.');

                        if (ageSplit.Length > 0)
                        {
                            patientAgeYear = ageSplit[0];
                            patientAgeYear = patientAgeYear.Length > 3 ? patientAgeYear.Substring(0, 3) : patientAgeYear;
                        }
                        if (ageSplit.Length > 1)
                        {
                            patientAgeMonth = ageSplit[1];
                            patientAgeMonth = patientAgeMonth.Length > 2 ? patientAgeMonth.Substring(0, 2) : patientAgeMonth;
                        }

                    }

                    foreach (var test in testlist)
                    {
                        await LisContext.LisDOM.AcknowledgeSample(test.Id);
                        var dxcTest = new Dxc700Tests(fieldConfiguration)
                        {
                            TestNo = test.LISTestCode,
                            DilInfo = string.Empty
                        };
                        testlst.Add(dxcTest);
                    }
                }

                patientName = patientName.Length > 20 ? patientName.Substring(0, 20) : patientName;

                var response = new DxC700Response(fieldConfiguration)
                {
                    RackNo = string.Empty, //4
                    CupNo = string.Empty, //2
                    Type = specimen, //1 *
                    SampleNo = sampleNo, //4
                    SampleId = sampleId, //26
                    Dummy = string.Empty, //4
                    BlockIdNo = "0", //1
                    Sex = patientGender, //1
                    Year = patientAgeYear, //3
                    Month = patientAgeMonth, //2
                    OtherType = string.Empty, //1
                    PatientInfo = patientName, //20
                    PatientId = patientId,
                    Tests = new List<Dxc700Tests>()
                };

                foreach (var item in testlst)
                {
                    item.TestNo = item.TestNo;
                    item.DilInfo = "";
                    response.Tests.Add(item);
                }

                string bodySegment = $"{Strings.Chr(2)}{response.ToString()}{Strings.Chr(3)}";

                if (!port.IsOpen)
                {
                    port.Open();
                }
                Logger.Logger.LogInstance.LogDebug("DXC700 Header Segment '{0}' '{1}'", bodySegment.Length, bodySegment);
                WriteToPort(bodySegment);
                //Logger.Logger.LogInstance.LogDebug("DXC700 Trailer Segment '{0}'", trailerSegment);
                //WriteToPort(trailerSegment);

                //WriteToPort("" + (char)5);
                index = 0;

                Logger.Logger.LogInstance.LogDebug("DXC700 SendOrderData method completed");

            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("DXC700 SendOrderData method exception:", ex);
            }
        }

        private string GetSpecimen(string specimenName)
        {
            var specimen = string.Empty;
            //24 hr urine
            //SPOT URINE
            //URINE
            //URINE.
            if (specimenName.ToUpper().Contains("URINE"))
            {
                specimen = "U";
                return specimen;
            }

            //BLOOD / URIN
            //BLOOD
            //BLOOD / FLUID
            //EDTA WHOLE BLOOD
            //HEPARINISED BLOOD
            //WHOLE BLOOD
            if (specimenName.ToUpper().Contains("BLOOD"))
            {
                specimen = "W";
                return specimen;
            }

            //C.S.F.
            //CITRATED PLASMA
            //FLUID
            //FLUID.
            //NULL
            //PLASMA
            //PLASMA / SERUM
            //SEMEN
            //SERUM
            //STONE
            //STOOL
            return specimen;

        }

        public override async Task Identify(string message)
        {
            message = message.TrimStart((char)2).TrimEnd((char)3);
            Logger.Logger.LogInstance.LogDebug("DXC700 Identify method started '{0}'", message);
            try
            {
                if (message.Length > 1)
                {
                    string type = message.GetValue(0, 2);
                    Logger.Logger.LogInstance.LogDebug("Type '{0}'", type);
                    switch (type)
                    {
                        case "D ":
                            await ParseMessageD(message);
                            break;
                        case "R ":
                            await ParseMessageR(message);
                            break;
                        case "DQ":
                            await ParseMessageDQ(message);
                            break;
                        default:
                            break;
                    }

                }
                Logger.Logger.LogInstance.LogDebug("DXC700 Identify method completed");
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("Identify method exception:", ex);
            }
        }

        private async Task ParseMessageR(string message)
        {
            Logger.Logger.LogInstance.LogDebug("DXC700 ParseMessageR method started '{0}'", message);

            var request = new DxC700Request(fieldConfiguration);
            request.ProcessMessage(message);

            Logger.Logger.LogInstance.LogDebug("DXC700 ParseMessageR before SendOrderData '{0}' '{1}'", request.SampleNo, request.SampleId);
            await SendOrderData(request.SampleId, request.SampleNo);

            Logger.Logger.LogInstance.LogDebug("DXC700 ParseMessageR method completed");
        }

        private async Task ParseMessageD(string message)
        {
            try
            {
                Logger.Logger.LogInstance.LogDebug("DXC700 ParseMessage method started '{0}'", message);

                var result = new DxC700Result(fieldConfiguration);
                result.ProcessMessage(message);
                foreach (var item in result.ResultDetails)
                {
                    Result newresult = new Result();
                    var testResult = result.TestResult;
                    testResult.LISTestCode = item.LISParamCode;

                    var resultDetails = new List<TestResultDetails>();
                    resultDetails.Add(item);

                    newresult.TestResult = testResult;
                    newresult.ResultDetails = resultDetails;

                    await LisContext.LisDOM.SaveTestResult(newresult);
                }

                Logger.Logger.LogInstance.LogDebug("DXC700 ParseMessage method completed");
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("DXC700 ParseMessage method exception:", ex);
            }
        }

        private async Task ParseMessageDQ(string message)
        {
            try
            {
                Logger.Logger.LogInstance.LogDebug("DXC700 ParseMessage method started '{0}'", message);

                var result = new DxC700ControlResult(fieldConfiguration);
                result.ProcessMessage(message);
                await LisContext.LisDOM.SaveTestResult(result);
                Logger.Logger.LogInstance.LogDebug("DXC700 ParseMessage method completed");
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("DXC700 ParseMessage method exception:", ex);
            }
        }
    }
}