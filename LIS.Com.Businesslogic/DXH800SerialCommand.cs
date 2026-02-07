using LIS.DtoModel;
using LIS.DtoModel.Models;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LIS.Com.Businesslogic
{
    public class DxH800SerialCommand : SerialCommand
    {
        public DxH800SerialCommand(PortSettings settings)
            : base(settings)
        {

        }

        public override async Task CreateMessage(string message)
        {
            Logger.Logger.LogInstance.LogDebug("DXH800 CreateMessage method started '{0}'", message);
            sInputMsg = "";
            string formattedmessage = "";
            string[] segments;
            try
            {
                segments = message.Split(Strings.Chr(10));  // Chr(10)
                for (int i = 0; i <= segments.Length - 1; i++)
                {
                    for (int j = 2; j <= segments[i].Length - 5; j++)
                    {
                        if (j != segments[i].Length - 5 | segments[i].ToString()[j + 1] != Strings.Chr(23))
                            formattedmessage += segments[i][j];
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("Create method exception:", ex);
            }

            await Identify(formattedmessage);
            Logger.Logger.LogInstance.LogDebug("DXH800 CreateMessage method completed");
        }
        public override async Task SendOrderData(string sampleId, string messageControlId)
        {
            try
            {
                data = new string[6];
                Logger.Logger.LogInstance.LogDebug("DXH800 SendOrderData method started for SampleNo: " + sampleId);

                string datetime = DateTime.Now.AddMinutes(-30).ToString("yyyyMMddhhmmss");
                var specialchar = @"\!~";
                var headerSegment = $"1H|{specialchar}|||LISHOST|||||||P|LIS2-A|{datetime}{Constants.vbCr}{Strings.Chr(3)}";

                var trailerSegment = "";
                var orderSegment = $"3O|1|{sampleId}||";
                IEnumerable<TestRequestDetail> testlist = await LisContext.LisDOM.GetTestRequestDetails(sampleId);

                if (testlist != null && testlist.Count() > 0)
                {
                    string patientFirstName = "";
                    string patientLastName = "";
                    string patientMiddleName = "";
                    trailerSegment = $"4L|1|N{Constants.vbCr}{Strings.Chr(3)}";

                    var firstTest = testlist.First();
                    var testname = "";
                    var specimen = string.Empty;
                    var patientGender = string.Empty;
                    var patientName = string.Empty;
                    var patientId = string.Empty;
                    var patientAgeYear = string.Empty;
                    var patientAgeMonth = string.Empty;

                    specimen = firstTest.SpecimenName;

                    patientId = firstTest.Patient?.HisPatientId.ToString();
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
                    }
                    var name = firstTest.Patient?.Name.Split(' ');
                    if (name.Count() > 1)
                    {
                        if (name.Count() == 4)
                        {
                            patientFirstName = name[1];
                            patientMiddleName = name[2];
                            patientLastName = name[3];
                        }
                        else if (name.Count() == 3)
                        {
                            patientFirstName = name[0];
                            patientMiddleName = name[1];
                            patientLastName = name[2];
                        }
                        else if (name.Count() == 2)
                        {
                            patientFirstName = name[0];
                            patientLastName = name[1];
                        }
                        else
                        {
                            patientFirstName = name[0];
                            patientMiddleName = name[1];
                            patientLastName = name[2];
                        }
                    }
                    else
                    {
                        patientFirstName = firstTest.Patient?.Name;
                    }

                    if (patientFirstName.Length > 20)
                    {
                        patientFirstName = patientFirstName.Substring(0, 19);
                    }
                    if (patientMiddleName.Length > 20)
                    {
                        patientMiddleName = patientMiddleName.Substring(0, 19);
                    }
                    if (patientLastName.Length > 20)
                    {
                        patientLastName = patientLastName.Substring(0, 19);
                    }

                    for (int i = 0; i < testlist.Count();)
                    {
                        var test = testlist.ElementAt(i);
                        var ackSent = await LisContext.LisDOM.AcknowledgeSample(test.Id);
                        testname += "!!!" + test.LISTestCode;
                        i++;
                        if (testlist.Count() == i)
                            break;
                        else
                            testname += @"\";
                    }

                    string patientSegment = $"2P|1||{patientId}||{patientLastName}!{patientFirstName}!{patientMiddleName}||!{patientAgeYear}!Y|{patientGender}|{Constants.vbCr}{Strings.Chr(3)}";
                    orderSegment += $"{testname}|R||||||||||{specimen}|{Constants.vbCr}{Strings.Chr(3)}";

                    data[0] = Strings.Chr(5).ToString();
                    data[1] = headerSegment;
                    Logger.Logger.LogInstance.LogDebug("DXH800 Header Segment {0}", headerSegment);
                    data[2] = patientSegment;
                    Logger.Logger.LogInstance.LogDebug("DXH800 Patient Segment {0}", patientSegment);
                    data[3] = orderSegment;
                    Logger.Logger.LogInstance.LogDebug("DXH800 Order Segment {0}", orderSegment);
                    data[4] = trailerSegment;
                    Logger.Logger.LogInstance.LogDebug("DXH800 Trailer Segment {0}", trailerSegment);
                    index = 0;
                }
                else//no test order
                {
                    headerSegment = $"1H|{specialchar}|{messageControlId}||LISHOST|||||||P|1{Constants.vbCr}{Strings.Chr(3)}";
                    trailerSegment = $"2L|1|F{Constants.vbCr}{Strings.Chr(3)}";
                    data[2] = Strings.Chr(5).ToString();
                    data[3] = headerSegment;
                    Logger.Logger.LogInstance.LogDebug("DXH800 Header Segment {0}", headerSegment);
                    data[4] = trailerSegment;
                    Logger.Logger.LogInstance.LogDebug("DXH800 Trailer Segment {0}", trailerSegment);
                    index = 2;
                }

                if (!port.IsOpen)
                {
                    port.Open();
                }
                WriteToPort("" + (char)5);

                Logger.Logger.LogInstance.LogDebug("DXH800 SendOrderData method completed for SampleNo " + sampleId);
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("DXH800 SendOrderData method exception:", ex);
            }
        }

        public override async Task Identify(string message)
        {
            Logger.Logger.LogInstance.LogDebug("DXH800 Identify method started");
            Logger.Logger.LogInstance.LogDebug("DXH800 Identify method Data: " + message);
            List<string> sampleList = new List<string>();
            ArrayList uniqueSampleList;
            string[] segments = message.Split(Strings.Chr(13)); // Chr(13)
            try
            {
                if (segments.Length > 1)
                {
                    if (segments[1].Substring(0, 1).ToUpper() == "Q")
                    {
                        string[] queryFields = segments[1].Split('|');
                        string[] sampleField = queryFields[2].Split('!');
                        string sampleID = sampleField[1];
                        string[] headerFields = segments[0].Split('|');
                        string messageControlID = headerFields[2];
                        await SendOrderData(sampleID, messageControlID);
                    }

                    else if (segments[1].Substring(0, 1).ToUpper() == "P")
                    {
                        for (int i = 0; i <= segments.Length - 2; i++)
                        {
                            if (segments[i].Substring(0, 1).ToUpper() == "O")
                            {
                                string sSpecimenId = segments[i].Split('|')[2];
                                sampleList.Add(sSpecimenId);
                            }
                        }

                        Hashtable ht = new Hashtable();
                        foreach (string str in sampleList)
                            ht[str] = DBNull.Value;

                        uniqueSampleList = new ArrayList(ht.Keys);
                        await ParseMessage(message, uniqueSampleList);
                    }
                }
                Logger.Logger.LogInstance.LogDebug("DXH800 Identify method completed");
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("DXH800 Identify method exception:", ex);
            }
        }

        public override async Task ParseMessage(string message, ArrayList sampleIdLst)
        {
            try
            {
                Logger.Logger.LogInstance.LogDebug("DXH800 ParseMessage method started");

                string[] record = message.Split(Strings.Chr(13)); // Chr(13)
                for (int j = 0; j <= sampleIdLst.Count - 1; j++)
                {
                    var result = new Result();
                    var lsResult = new List<TestResultDetails>();
                    var testResult = new TestResult();
                    string lisTestCode = "";
                    testResult.ResultDate = DateAndTime.Now;
                    string sampleNo = "";
                    for (int index = 0; index <= record.Length - 1; index++)
                    {
                        string[] field = record[index].Split('|');
                        switch (field[0])
                        {
                            case "O":
                                {
                                    sampleNo = field[2];
                                    testResult.SampleNo = sampleNo;
                                    if (field[4].Length > 0)
                                    {
                                        lisTestCode = field[4].Split('!')[3];
                                        testResult.LISTestCode = lisTestCode.Trim('\\');
                                    }
                                    break;
                                }

                            case "R":
                                {
                                    if (sampleNo == sampleIdLst[j].ToString())
                                    {
                                        var resultDetails = new TestResultDetails
                                        {
                                            LISParamCode = field[2].Split('!')[3],
                                            LISParamValue = field[3].Split('!')[0],
                                            LISParamUnit = field[4]
                                        };
                                        lsResult.Add(resultDetails);
                                    }
                                    break;
                                }
                        }
                    }

                    //lsResult = CalculateParameterResult(lsResult);
                    result.TestResult = testResult;
                    result.ResultDetails = lsResult;
                    Logger.Logger.LogInstance.LogDebug("DXH800 Result posted to API for SampleNo: " + testResult.SampleNo);
                    await LisContext.LisDOM.SaveTestResult(result);

                }
                Logger.Logger.LogInstance.LogDebug("DXH800 ParseMessage method completed");
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("DXH800 ParseMessage method exception:", ex);
            }
        }

        //private List<TestResultDetails> CalculateParameterResult(List<TestResultDetails> lsResult)
        //{
        //    var wbc = getValueFromList(lsResult, "WBC");
        //    var mop = getValueFromList(lsResult, "MO");
        //    var bap = getValueFromList(lsResult, "BA");
        //    var eop = getValueFromList(lsResult, "EO");
        //    var lyp = getValueFromList(lsResult, "LY");
        //    decimal nep, nen, lyn = 0;
        //    if (!string.IsNullOrEmpty(wbc) && !string.IsNullOrEmpty(mop) && !string.IsNullOrEmpty(bap) && !string.IsNullOrEmpty(eop))
        //    {
        //        nep = 100 - (Convert.ToDecimal(mop) + Convert.ToDecimal(bap) + Convert.ToDecimal(eop) + Convert.ToDecimal(lyp));
        //        nen = Convert.ToDecimal(wbc) * nep / 100;
        //        lyn = Convert.ToDecimal(wbc) * Convert.ToDecimal(lyp) / 100;

        //        //Update NE% value
        //        lsResult.Where(w => w.LISParamCode.Equals("NE")).ToList().ForEach(s => s.LISParamValue = nep.ToString());
        //        //Update NE# value
        //        lsResult.Where(w => w.LISParamCode.Equals("NE#")).ToList().ForEach(s => s.LISParamValue = nen.ToString());
        //        //Update LY# value
        //        lsResult.Where(w => w.LISParamCode.Equals("LY#")).ToList().ForEach(s => s.LISParamValue = lyn.ToString());
        //    }
        //    return lsResult;
        //}

        //private string getValueFromList(List<TestResultDetails> lsResult, string code)
        //{
        //    var result = lsResult.Find(p => p.LISParamCode.Equals(code));
        //    if (result != null)
        //    {
        //        return result.LISParamValue;
        //    }
        //    return null;
        //}

        //private TestResultDetails GetParameterResult(string[] field)
        //{
        //    TestResultDetails resdt = new TestResultDetails();
        //    var paramCode = field[2].Split('!')[3];
        //    string paramValue;
        //    string paramUnit;
        //    switch (paramCode)
        //    {               
        //        case "PLT":
        //        case "RET#":
        //        case "NRBC#":
        //        case "MO#":
        //        case "EO#":
        //        case "BA#":
        //            var actualValue = field[3].Split('!')[0];
        //            if (!string.IsNullOrWhiteSpace(actualValue))
        //            {
        //                try
        //                {
        //                    paramValue = (Convert.ToDecimal(actualValue) * 1000).ToString();
        //                }
        //                catch (Exception)
        //                {
        //                    paramValue = "";
        //                }
        //            }
        //            else
        //            {
        //                paramValue = "";
        //            }

        //            paramUnit = field[4];
        //            break;
        //        default:
        //            try
        //            {
        //                paramValue = Convert.ToDecimal(field[3].Split('!')[0]).ToString();
        //            }
        //            catch (Exception)
        //            {
        //                paramValue = "";
        //            }
        //            paramUnit = field[4];
        //            break;

        //    }

        //    resdt.LISParamCode = paramCode;
        //    resdt.LISParamValue = paramValue;
        //    resdt.LISParamUnit = paramUnit;

        //    return resdt;
        //}
    }
}