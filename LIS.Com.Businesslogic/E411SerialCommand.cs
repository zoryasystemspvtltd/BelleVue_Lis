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
    /// <summary>
    /// This class is implented Elecsys format for Cobas E411
    /// </summary>
    public class E411SerialCommand : SerialCommand
    {
        public E411SerialCommand(PortSettings settings)
            : base(settings)
        {

        }

        public override async Task CreateMessage(string message)
        {
            Logger.Logger.LogInstance.LogDebug("E411 CreateMessage method started. '{0}'", message);
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
                Logger.Logger.LogInstance.LogException("E411 CreateMessage method exception:", ex);
            }
            await Identify(formattedmessage);
            Logger.Logger.LogInstance.LogDebug("E411 CreateMessage method completed");
        }
        public override async Task SendOrderData(string queryFields)
        {
            try
            {               
                string datetime = DateTime.Now.AddMinutes(-30).ToString("yyyyMMddhhmmss");
                string[] sampleField = queryFields.Split('^');
                string sampleId = sampleField[1];
                string sequenceNo = sampleField[2];
                string carrierNo = sampleField[3];
                string positionNo = sampleField[4];
                string sampleType = sampleField[6];
                string containerType = sampleField[7];
                string rangId = sequenceNo + "^" + carrierNo + "^" + positionNo + "^^" + sampleType + "^" + containerType + "^R1";
                Logger.Logger.LogInstance.LogDebug("E411 SendOrderData method started for SampleNo: " + sampleId);

                string specialchar = @"\^&";
                string headerSegment = $"1H|{specialchar}|||cobas-e411^1|||||host|TSDWN^REPLY|P|1{Constants.vbCr}{Strings.Chr(3)}";
                string patientSegment = "";
                string orderSegment = "";
                string trailerSegment = $"4L|1|N{Constants.vbCr}{Strings.Chr(3)}";
                IEnumerable<TestRequestDetail> testlist = await LisContext.LisDOM.GetTestRequestDetails(sampleId);
                string reportType = "Z";
                //var specimen = "";
                //var patientName = "";
                //var patientId = "";
                var testname = "";
                if (testlist.Count() > 0)
                {
                    reportType = "O";                    

                    for (int i = 0; i < testlist.Count();)
                    {
                        var test = testlist.ElementAt(i);
                        var ackSent = await LisContext.LisDOM.AcknowledgeSample(test.Id);
                        testname += "^^^" + test.LISTestCode + "^";
                        i++;
                        if (testlist.Count() == i)
                            break;
                        else
                            testname += @"\";
                    }
                }

                patientSegment = $"2P|1|{Constants.vbCr}{Strings.Chr(3)}";
                orderSegment = $"3O|1|{sampleId}|{rangId}|{testname}|R||||||A||^^||||||^^^^||||||{reportType}{Constants.vbCr}{Strings.Chr(3)}";

                data[0] = Strings.Chr(5).ToString();
                data[1] = headerSegment;
                Logger.Logger.LogInstance.LogDebug("E411 Header Segment {0}", headerSegment);
                data[2] = patientSegment;
                Logger.Logger.LogInstance.LogDebug("E411 Patient Segment {0}", patientSegment);
                data[3] = orderSegment;
                Logger.Logger.LogInstance.LogDebug("E411 Order Segment {0}", orderSegment);
                data[4] = trailerSegment;
                Logger.Logger.LogInstance.LogDebug("E411 Trailer Segment {0}", trailerSegment);

                index = 0;

                if (!port.IsOpen)
                {
                    port.Open();
                }
                WriteToPort("" + (char)5);
                Logger.Logger.LogInstance.LogDebug("E411 SendOrderData method completed for SampleNo: " + sampleId);
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("E411 SendOrderData method exception:", ex);
            }
        }

        public override async Task Identify(string message)
        {
            Logger.Logger.LogInstance.LogDebug("E411 Identify method started");
            Logger.Logger.LogInstance.LogDebug("E411 Identify method Data:" + message);
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
                        //string[] sampleField = queryFields[2].Split('^');
                        //string sampleID = sampleField[1];
                        await SendOrderData(queryFields[2]);
                    }
                    else if (segments[1].Substring(0, 1).ToUpper() == "P")
                    {
                        Logger.Logger.LogInstance.LogDebug("Patient Info started");
                        for (int i = 0; i <= segments.Length - 2; i++)
                        {
                            if (segments[i].Substring(0, 1).ToUpper() == "O")
                            {
                                string sSpecimenId = segments[i].Split('|')[2];
                                sampleList.Add(sSpecimenId.Split('^')[0]);
                            }
                        }

                        Hashtable ht = new Hashtable();
                        foreach (string str in sampleList)
                            ht[str] = DBNull.Value;

                        uniqueSampleList = new ArrayList(ht.Keys);
                        await ParseMessage(message, uniqueSampleList);
                    }
                }
                Logger.Logger.LogInstance.LogDebug("E411 Identify method completed");
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("E411 Identify method exception:", ex);
            }
        }

        public override async Task ParseMessage(string message, ArrayList sampleIdLst)
        {
            try
            {
                Logger.Logger.LogInstance.LogDebug("E411 ParseMessage method started");
                Logger.Logger.LogInstance.LogDebug("E411 ParseMessage method Data: " + message);
                string[] record = message.Split(Strings.Chr(13)); // Chr(13)
                for (int j = 0; j <= sampleIdLst.Count - 1; j++)
                {
                    Result result = new Result();
                    List<TestResultDetails> lsResult = new List<TestResultDetails>();
                    TestResult testResult = new TestResult();
                    testResult.ResultDate = DateAndTime.Now;
                    string sampleNo = "";
                    bool isPanel = false;
                    for (int index = 0; index <= record.Length - 1; index++)
                    {
                        string[] field = record[index].Split('|');
                        switch (field[0])
                        {
                            case "O":
                                {
                                    sampleNo = field[2];
                                    testResult.SampleNo = sampleNo;
                                    break;
                                }

                            case "R":
                                {
                                    if (sampleNo == sampleIdLst[j].ToString())
                                    {
                                        TestResultDetails resultDetails = new TestResultDetails();
                                        string[] parameter = field[2].Split('^');
                                        string paramCode = parameter[3];
                                        testResult.LISTestCode = paramCode;
                                        if (paramCode != "")
                                        {
                                            resultDetails.LISParamCode = paramCode;
                                            resultDetails.LISParamUnit = field[4];

                                            string paramResult = field[3].Split('^')[0];
                                            if (paramResult == "1")
                                                paramResult = "Positive";
                                            else if (paramResult == "0")
                                                paramResult = "Border line";
                                            else if (paramResult == "-1")
                                                paramResult = "Negative";

                                            resultDetails.LISParamValue = paramResult;
                                        }
                                        Logger.Logger.LogInstance.LogDebug("E411 Result processed for SampleNo " + sampleNo + " and Parameter " + paramCode);
                                        lsResult.Add(resultDetails);

                                        //isPanel test checking
                                        isPanel = await LisContext.LisDOM.IsPanelTest(sampleNo, paramCode);
                                        if (!isPanel)
                                        {
                                            TestResult newTestResult = new TestResult();
                                            newTestResult.ResultDate = DateAndTime.Now;
                                            newTestResult.LISTestCode = paramCode;
                                            newTestResult.SampleNo = sampleNo;
                                            result.TestResult = newTestResult;
                                            result.ResultDetails = lsResult;
                                            Logger.Logger.LogInstance.LogDebug("E411 Result posted to API for SampleNo: " + testResult.SampleNo);
                                            await LisContext.LisDOM.SaveTestResult(result);
                                            lsResult.Clear();
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                    if (isPanel)
                    {
                        result.TestResult = testResult;
                        result.ResultDetails = lsResult;
                        Logger.Logger.LogInstance.LogDebug("E411 Result posted to API for SampleNo: " + testResult.SampleNo);
                        await LisContext.LisDOM.SaveTestResult(result);
                    }
                }
                Logger.Logger.LogInstance.LogDebug("E411 ParseMessage method completed");
            }
            catch (Exception ex)
            {
                Logger.Logger.LogInstance.LogException("E411 ParseMessage method exception:", ex);
            }
        }
    }
}