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
    public class CA600SerialCommand : SerialCommand
    {
        public CA600SerialCommand(PortSettings settings)
            : base(settings)
        {

        }

        public override async Task CreateMessage(string message)
        {
            //Remove <CHK1>,<CHK2> character from raw message
            message = message.Replace("<CHK1>", "9");
            message = message.Replace("<CHK2>", "D");
            Logger.Logger.LogInstance.LogDebug("CA600 CreateMessage method started '{0}'", message);
            sInputMsg = "";
            string formattedmessage = "";
            string[] segments;
            try
            {               
                segments = message.Split(Strings.Chr(10));  // Chr(10) <LF>
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
            Logger.Logger.LogInstance.LogDebug("CA600 CreateMessage method completed");
        }
        public override async Task SendOrderData(string sampleStr)
        {
            try
            {
                string[] sampleField = sampleStr.Split('^');
                string sampleId = sampleField[2].Trim();
                Logger.Logger.LogInstance.LogDebug("CA600 SendOrderData method started for SampleNo: " + sampleId);

                string datetime = DateTime.Now.AddMinutes(-30).ToString("yyyyMMddhhmmss");
                var specialchar = @"\^&";
                var headerSegment = $"1H|{specialchar}|||LIS^^^^|||||CA-600{Strings.Chr(13)}<CHK1><CHK2>{Strings.Chr(3)}";
                string patientSegment = $"2P|1|{Strings.Chr(13)}<CHK1><CHK2>{Strings.Chr(3)}";
                var trailerSegment = "";
                var orderSegment = $"3O|1|{sampleStr}||";
                IEnumerable<TestRequestDetail> testlist = await LisContext.LisDOM.GetTestRequestDetails(sampleId);

                if (testlist != null && testlist.Count() > 0)
                {                  
                    trailerSegment = $"4L|1|N{Strings.Chr(13)}<CHK1><CHK2>{Strings.Chr(3)}";

                    var firstTest = testlist.First();
                    var testname = "";
                    for (int i = 0; i < testlist.Count();)
                    {
                        var test = testlist.ElementAt(i);
                        var ackSent = await LisContext.LisDOM.AcknowledgeSample(test.Id);
                        testname += "^^^" + test.LISTestCode+ "^^100";
                        i++;
                        if (testlist.Count() == i)
                            break;
                        else
                            testname += @"\";
                    }

                    orderSegment += $"{testname}|R|{datetime}|||||N{Strings.Chr(13)}<CHK1><CHK2>{Strings.Chr(3)}";

                    data[0] = Strings.Chr(5).ToString();
                    data[1] = headerSegment;
                    Logger.Logger.LogInstance.LogDebug("CA600 Header Segment {0}", headerSegment);
                    data[2] = patientSegment;
                    Logger.Logger.LogInstance.LogDebug("CA600 Patient Segment {0}", patientSegment);
                    data[3] = orderSegment;
                    Logger.Logger.LogInstance.LogDebug("CA600 Order Segment {0}", orderSegment);
                    data[4] = trailerSegment;
                    Logger.Logger.LogInstance.LogDebug("CA600 Trailer Segment {0}", trailerSegment);
                    index = 0;
                }
                else//no test order
                {
                    headerSegment = $"1H|{specialchar}|||||||||||E1394-97{Strings.Chr(13)}<CHK1><CHK2>{Strings.Chr(3)}";
                    patientSegment = $"2P|1{Strings.Chr(13)}<CHK1><CHK2>{Strings.Chr(3)}";
                    orderSegment = $"3O|{sampleStr}||||{datetime}||||{Strings.Chr(13)}<CHK1><CHK2>{Strings.Chr(3)}";
                    trailerSegment = $"4L|1|N{Strings.Chr(13)}<CHK1><CHK2>{Strings.Chr(3)}";
                    data[0] = Strings.Chr(5).ToString();
                    data[1] = headerSegment;
                    Logger.Logger.LogInstance.LogDebug("CA600 Header Segment {0}", headerSegment);
                    data[2] = patientSegment;
                    Logger.Logger.LogInstance.LogDebug("CA600 Patient Segment {0}", patientSegment);
                    data[3] = orderSegment;
                    Logger.Logger.LogInstance.LogDebug("CA600 Order Segment {0}", orderSegment);
                    data[4] = trailerSegment;
                    Logger.Logger.LogInstance.LogDebug("CA600 Trailer Segment {0}", trailerSegment);
                    index = 0;
                }

                if (!port.IsOpen)
                {
                    port.Open();
                }
                WriteToPort("" + (char)5);

                Logger.Logger.LogInstance.LogDebug("CA600 SendOrderData method completed for SampleNo " + sampleId);
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("CA600 SendOrderData method exception:", ex);
            }
        }

        public override async Task Identify(string message)
        {
            Logger.Logger.LogInstance.LogDebug("CA600 Identify method started");
            Logger.Logger.LogInstance.LogDebug("CA600 Identify method Data: " + message);
            List<string> sampleList = new List<string>();
            ArrayList uniqueSampleList;            
            string[] segments = message.Split(Strings.Chr(13)); // Chr(13) <CR>
            try
            {
                if (segments.Length > 1)
                {
                    if (segments[1].Substring(0, 1).ToUpper() == "Q")
                    {
                        string[] queryFields = segments[1].Split('|');
                        await SendOrderData(queryFields[2]);
                    }

                    else if (segments[1].Substring(0, 1).ToUpper() == "P")
                    {
                        for (int i = 0; i <= segments.Length - 2; i++)
                        {
                            if (segments[i].Substring(0, 1).ToUpper() == "O")
                            {
                                string sSpecimenId = segments[i].Split('^')[2].Trim();
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
                Logger.Logger.LogInstance.LogDebug("CA600 Identify method completed");
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("CA600 Identify method exception:", ex);
            }
        }

        public override async Task ParseMessage(string message, ArrayList sampleIdLst)
        {
            try
            {
                Logger.Logger.LogInstance.LogDebug("CA600 ParseMessage method started");

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
                                    string[] sampleField = field[2].Split('^');
                                    sampleNo = sampleField[2].Trim();
                                    testResult.SampleNo = sampleNo;
                                    if (field[4].Length > 0)
                                    {
                                        lisTestCode = field[4].Split('^')[3];
                                        testResult.LISTestCode = lisTestCode;
                                    }
                                    break;
                                }

                            case "R":
                                {
                                    if (sampleNo == sampleIdLst[j].ToString())
                                    {
                                        TestResultDetails resultDetails = new TestResultDetails();
                                        string[] parameter = field[2].Split('^');
                                        string paramCode = parameter[3];
                                        if (paramCode != "")
                                        {
                                            resultDetails.LISParamCode = paramCode;
                                            resultDetails.LISParamValue = field[3];
                                            resultDetails.LISParamUnit = field[4];

                                        }
                                        Logger.Logger.LogInstance.LogDebug("CA600 Result processed for SampleNo " + sampleNo + " and Parameter " + paramCode);
                                        lsResult.Add(resultDetails);
                                    }
                                    break;
                                }
                        }
                    }

                    result.TestResult = testResult;
                    result.ResultDetails = lsResult;
                    Logger.Logger.LogInstance.LogDebug("CA600 Result posted to API for SampleNo: " + testResult.SampleNo);
                    await LisContext.LisDOM.SaveTestResult(result);

                }
                Logger.Logger.LogInstance.LogDebug("CA600 ParseMessage method completed");
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("CA600 ParseMessage method exception:", ex);
            }
        }
    }
}