using System;
using System.Threading.Tasks;
using LIS.Com.Businesslogic;
using LIS.DtoModel;
using LIS.DtoModel.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Linq;
using System.Collections.Generic;

namespace LisConsoleTest
{
    [TestClass]
    public class DxC700SerailCommandTest
    {
        DxC700Configuration configuration = new DxC700Configuration()
        {
            RackNo = new Field() { Enabled = true, Lenght = 4 },//4
            CupNo = new Field() { Enabled = true, Lenght = 2 },//2
            Type = new Field() { Enabled = true, Lenght = 1 },//1
            SampleNo = new Field() { Enabled = true, Lenght = 4 },//4
            
            SampleId = new Field() { Enabled = true, Lenght = 26 },//26
                                                                   //First Run Sample No. 4
            Dummy = new Field() { Enabled = true, Lenght = 4 },//4
            BlockIdNo = new Field() { Enabled = true, Lenght = 1 },//1
            Sex = new Field() { Enabled = true, Lenght = 1 },//1
            Year = new Field() { Enabled = true, Lenght = 3 },//3
            Month = new Field() { Enabled = true, Lenght = 2 },//2
            OtherType = new Field() { Enabled = true, Lenght = 1 },//1
            PatientInfo = new Field() { Enabled = true, Lenght = 20 },//20 This could be repeated
            PatientId = new Field() { Enabled = true, Lenght = 20 },//20
            RunDateTime = new Field() { Enabled = false, Lenght = 14 },//14
            OnlineTestNumber = new Field() { Enabled = false, Lenght = 3 },//3
            DilutionInfo = new Field() { Enabled = false, Lenght = 1 },//Dilution Info. 1
            ReagentInfo = new Field() { Enabled = false, Lenght = 16 },// 4*4 = 16
            ParamCode = new Field() { Enabled = true, Lenght = 3 },//3
            ParamValue = new Field() { Enabled = true, Lenght = 6 },//6
            Flags = new Field() { Enabled = true, Lenght = 2 },
            ISEElectrode = new Field() { Enabled = false, Lenght = 20 },

            // For Test
            TestNo = new Field() { Enabled = true, Lenght = 3 },//3
            DilInfo = new Field() { Enabled = false, Lenght = 1 },//1

            // For Control
            ControlNo = new Field() { Enabled = true, Lenght = 4 },//4
            
        };

        [TestMethod]
        public void Parse_Test_Request_Message()
        {
            var s = new List<string>();
            //var json = JsonConvert.SerializeObject(configuration);

            var dxc = new DxC700Request(configuration);

            var message = "R       N0001                    ZBR010";
            dxc.ProcessMessage(message);

            Assert.AreEqual("", dxc.RackNo);
            Assert.AreEqual("", dxc.CupNo);
            Assert.AreEqual("N", dxc.Type);
            Assert.AreEqual("0001", dxc.SampleNo);
            Assert.AreEqual("ZBR010", dxc.SampleId);
        }

        [TestMethod]
        public void Generate_Response_String()
        {
            var response = new DxC700Response(configuration)
            {
                RackNo = "",
                CupNo = "",
                Type = "",
                SampleNo = "0001",
                SampleId = "ZBR010",
                Dummy = "",
                BlockIdNo = "0",
                Sex = "M",
                Year = "",
                Month = "08",
                OtherType = "",
                PatientInfo = "ZBR010",
                PatientId = "1234",
                Tests = new List<Dxc700Tests>()
                {
                    new Dxc700Tests(configuration)
                    {
                        TestNo = "001",
                        DilInfo = ""
                    }
                }
            };

            var result = response.ToString();
            var expected = "S        0001                    ZBR010    EM00008 ZBR010              1234                001";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Parse_Test_Result_Message_Single()
        {
            var dxc = new DxC700Result(configuration);
            var message = "D     01 P001                   1505073    E0      B.MAJUMDER/S1       1505073             001    97  ";
            dxc.ProcessMessage(message);

            Assert.AreEqual("1505073", dxc.TestResult.SampleNo);
            Assert.AreEqual("001", dxc.TestResult.LISTestCode);
            Assert.AreEqual(1, dxc.ResultDetails.Count());
            var result = dxc.ResultDetails.First();
            Assert.AreEqual("001", result.LISParamCode);
            Assert.AreEqual("97", result.LISParamValue);
        }

        [TestMethod]
        public void Parse_Test_Result_Message_Multiple()
        {
            var dxc = new DxC700Result(configuration);
            //var message = "D 000110 0110                   1416235    E0      S.N.DAS             1478104             009  5.31                      010  3.09                      023   2.2                      024   1.4                      057  2.03                      ";
            var message = "D     10 P010                   1501835    E0      S.M NANDI                               097 134.2r 098  2.39r ";
            dxc.ProcessMessage(message);

            Assert.AreEqual("1501835", dxc.TestResult.SampleNo);
            Assert.AreEqual("098", dxc.TestResult.LISTestCode);
            Assert.AreEqual(2, dxc.ResultDetails.Count());

            var result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("097"));
            Assert.AreEqual("134.2", result.LISParamValue);

            result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("098"));
            Assert.AreEqual("2.39", result.LISParamValue);

           
        }

        [TestMethod]
        public void Parse_Control_Result_Message()
        {
            var dxc = new DxC700ControlResult(configuration);
            var message = "DQ       Q001                       QC2 002E001   291  005   106  006   101  012    98  016  4.30  020  7.87  097 124.8  098  6.05  099    85  028 467.9  030   385  ";
            dxc.ProcessMessage(message);

            Assert.AreEqual("QC2", dxc.TestResult.SampleNo);
            Assert.AreEqual(11, dxc.ResultDetails.Count());

            var result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("001"));
            Assert.AreEqual("291", result.LISParamValue);

            result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("005"));
            Assert.AreEqual("106", result.LISParamValue);

            result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("006"));
            Assert.AreEqual("101", result.LISParamValue);

            result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("012"));
            Assert.AreEqual("98", result.LISParamValue);

            result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("016"));
            Assert.AreEqual("4.30", result.LISParamValue);

            result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("020"));
            Assert.AreEqual("7.87", result.LISParamValue);

            result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("097"));
            Assert.AreEqual("124.8", result.LISParamValue);

            result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("098"));
            Assert.AreEqual("6.05", result.LISParamValue);

            result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("099"));
            Assert.AreEqual("85", result.LISParamValue);

            result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("028"));
            Assert.AreEqual("467.9", result.LISParamValue);

            result = dxc.ResultDetails.FirstOrDefault(p => p.LISParamCode.Equals("030"));
            Assert.AreEqual("385", result.LISParamValue);
        }
    }
}
