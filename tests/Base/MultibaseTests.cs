using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Multiformats.Base.Tests
{
    public class MultibaseTests
    {
        [Test]
        public void Decode_GivenInvalidChars_ThrowsInvalidOperationException()
        {
            // prefix 0 - base2
            // value 99 - invalid chars

            Assert.Throws<InvalidOperationException>(() => Multibase.Decode("099", out string _));
        }

        [Theory]
        [TestCase("")]
        [TestCase(null)]
        public void Decode_GivenEmptyInput_ThrowsArgumentNullException(string value)
        {
            Assert.Throws<ArgumentNullException>(() => Multibase.Decode(value, out string _));
        }

        [Test]
        public void Decode_GivenUnknownPrefix_ThrowsUnsupportedException()
        {
            Assert.Throws<NotSupportedException>(() => Multibase.Decode("ø", out string _));
        }

        [Test]
        public void DecodeRaw_GivenUnknownEncoding_ThrowsUnsupportedException()
        {
            Assert.Throws<NotSupportedException>(() => Multibase.DecodeRaw((MultibaseEncoding)0x2000, "abab"));
        }

        [Theory]
        [TestCase("")]
        [TestCase(null)]
        public void DecodeRaw_GivenEmptyInput_ThrowsArgumentNullException(string value)
        {
            Assert.Throws<ArgumentNullException>(() => Multibase.DecodeRaw(MultibaseEncoding.Identity, value));
        }

        [Test]
        public void Encode_GivenEmptyBytes_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Multibase.Encode(MultibaseEncoding.Base2, new byte[] { }));
        }

        [Test]
        public void Encode_GivenNullBytes_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Multibase.Encode(MultibaseEncoding.Base2, null));
        }

        [Test]
        public void Encode_GivenUnknownEncoding_ThrowsUnsupportedException()
        {
            Assert.Throws<NotSupportedException>(() => Multibase.Encode((MultibaseEncoding)0x2000, new byte[] { 0, 1, 2, 3 }));
        }

        [Theory]
        [TestCase(MultibaseEncoding.Identity)]
        [TestCase(MultibaseEncoding.Base2)]
        [TestCase(MultibaseEncoding.Base8)]
        [TestCase(MultibaseEncoding.Base10)]
        [TestCase(MultibaseEncoding.Base16Lower)]
        [TestCase(MultibaseEncoding.Base16Upper)]
        [TestCase(MultibaseEncoding.Base32Lower)]
        [TestCase(MultibaseEncoding.Base32Upper)]
        [TestCase(MultibaseEncoding.Base32PaddedLower)]
        [TestCase(MultibaseEncoding.Base32PaddedUpper)]
        [TestCase(MultibaseEncoding.Base32HexLower)]
        [TestCase(MultibaseEncoding.Base32HexUpper)]
        [TestCase(MultibaseEncoding.Base32HexPaddedLower)]
        [TestCase(MultibaseEncoding.Base32HexPaddedUpper)]
        [TestCase(MultibaseEncoding.Base58Btc)]
        [TestCase(MultibaseEncoding.Base58Flickr)]
        [TestCase(MultibaseEncoding.Base64)]
        [TestCase(MultibaseEncoding.Base64Padded)]
        [TestCase(MultibaseEncoding.Base64Url)]
        [TestCase(MultibaseEncoding.Base64UrlPadded)]
        public void TestRoundTrip(MultibaseEncoding encoding)
        {
            var rand = new Random(Environment.TickCount);
            var buf = new byte[rand.Next(16, 256)];
            rand.NextBytes(buf);

            var encoded = Multibase.EncodeRaw(encoding, buf);
            var decoded = Multibase.DecodeRaw(encoding, encoded);

            Assert.AreEqual(decoded, buf);
        }

        [Theory]
        [TestCase(MultibaseEncoding.Identity)]
        [TestCase(MultibaseEncoding.Base2)]
        [TestCase(MultibaseEncoding.Base8)]
        [TestCase(MultibaseEncoding.Base10)]
        [TestCase(MultibaseEncoding.Base16Lower)]
        [TestCase(MultibaseEncoding.Base16Upper)]
        [TestCase(MultibaseEncoding.Base32Lower)]
        [TestCase(MultibaseEncoding.Base32Upper)]
        [TestCase(MultibaseEncoding.Base32PaddedLower)]
        [TestCase(MultibaseEncoding.Base32PaddedUpper)]
        [TestCase(MultibaseEncoding.Base32HexLower)]
        [TestCase(MultibaseEncoding.Base32HexUpper)]
        [TestCase(MultibaseEncoding.Base32HexPaddedLower)]
        [TestCase(MultibaseEncoding.Base32HexPaddedUpper)]
        [TestCase(MultibaseEncoding.Base58Btc)]
        [TestCase(MultibaseEncoding.Base58Flickr)]
        [TestCase(MultibaseEncoding.Base64)]
        [TestCase(MultibaseEncoding.Base64Padded)]
        [TestCase(MultibaseEncoding.Base64Url)]
        [TestCase(MultibaseEncoding.Base64UrlPadded)]
        public void TestRoundTripRaw(MultibaseEncoding encoding)
        {
            var rand = new Random(Environment.TickCount);
            var buf = new byte[rand.Next(16, 256)];
            rand.NextBytes(buf);

            var encoded = Multibase.Encode(encoding, buf);
            var decoded = Multibase.Decode(encoded, out MultibaseEncoding decodedEncoding);

            Assert.That(decodedEncoding, Is.EqualTo(encoding));
            Assert.That(buf, Is.EqualTo(decoded));
        }

        [Theory]
        [TestCase("001000100011001010110001101100101011011100111010001110010011000010110110001101001011110100110010100100000011001010111011001100101011100100111100101110100011010000110100101101110011001110010000100100001", MultibaseEncoding.Base2)]
        [TestCase("71043126154533472162302661513646244031273145344745643206455631620441", MultibaseEncoding.Base8)]
        [TestCase("f446563656e7472616c697a652065766572797468696e672121", MultibaseEncoding.Base16Lower)]
        [TestCase("F446563656E7472616C697A652065766572797468696E672121", MultibaseEncoding.Base16Upper)]
        [TestCase("birswgzloorzgc3djpjssazlwmvzhs5dinfxgoijb", MultibaseEncoding.Base32Lower)]
        [TestCase("BIRSWGZLOORZGC3DJPJSSAZLWMVZHS5DINFXGOIJB", MultibaseEncoding.Base32Upper)]
        [TestCase("v8him6pbeehp62r39f9ii0pbmclp7it38d5n6e891", MultibaseEncoding.Base32HexLower)]
        [TestCase("V8HIM6PBEEHP62R39F9II0PBMCLP7IT38D5N6E891", MultibaseEncoding.Base32HexUpper)]
        [TestCase("cirswgzloorzgc3djpjssazlwmvzhs5dinfxgoijb", MultibaseEncoding.Base32PaddedLower)]
        [TestCase("CIRSWGZLOORZGC3DJPJSSAZLWMVZHS5DINFXGOIJB", MultibaseEncoding.Base32PaddedUpper)]
        [TestCase("t8him6pbeehp62r39f9ii0pbmclp7it38d5n6e891", MultibaseEncoding.Base32HexPaddedLower)]
        [TestCase("T8HIM6PBEEHP62R39F9II0PBMCLP7IT38D5N6E891", MultibaseEncoding.Base32HexPaddedUpper)]
        [TestCase("het1sg3mqqt3gn5djxj11y3msci3817depfzgqejb", MultibaseEncoding.Base32Z)]
        [TestCase("Ztwe7gVTeK8wswS1gf8hrgAua9fcw9reboD", MultibaseEncoding.Base58Flickr)]
        [TestCase("zUXE7GvtEk8XTXs1GF8HSGbVA9FCX9SEBPe", MultibaseEncoding.Base58Btc)]
        [TestCase("mRGVjZW50cmFsaXplIGV2ZXJ5dGhpbmchIQ", MultibaseEncoding.Base64)]
        [TestCase("MRGVjZW50cmFsaXplIGV2ZXJ5dGhpbmchIQ==", MultibaseEncoding.Base64Padded)]
        [TestCase("uRGVjZW50cmFsaXplIGV2ZXJ5dGhpbmchIQ", MultibaseEncoding.Base64Url)]
        [TestCase("URGVjZW50cmFsaXplIGV2ZXJ5dGhpbmchIQ==", MultibaseEncoding.Base64UrlPadded)]
        public void TestTryDecoding_GivenValidEncodedInput_Prefixed(string input, MultibaseEncoding encoding)
        {
            var expected = "Decentralize everything!!";
            var result = Multibase.TryDecode(input, out var decodedEncoding, out var decodedBytes);

            Assert.True(result);
            Assert.AreEqual(encoding, decodedEncoding);
            Assert.AreEqual(expected, Encoding.UTF8.GetString(decodedBytes));
        }

        [Theory]
        [TestCase("01000100011001010110001101100101011011100111010001110010011000010110110001101001011110100110010100100000011001010111011001100101011100100111100101110100011010000110100101101110011001110010000100100001", MultibaseEncoding.Base2)]
        [TestCase("1043126154533472162302661513646244031273145344745643206455631620441", MultibaseEncoding.Base8)]
        [TestCase("446563656e7472616c697a652065766572797468696e672121", MultibaseEncoding.Base16Lower)]
        [TestCase("446563656E7472616C697A652065766572797468696E672121", MultibaseEncoding.Base16Upper)]
        [TestCase("irswgzloorzgc3djpjssazlwmvzhs5dinfxgoijb", MultibaseEncoding.Base32Lower)]
        [TestCase("IRSWGZLOORZGC3DJPJSSAZLWMVZHS5DINFXGOIJB", MultibaseEncoding.Base32Upper)]
        [TestCase("8him6pbeehp62r39f9ii0pbmclp7it38d5n6e891", MultibaseEncoding.Base32HexLower)]
        [TestCase("8HIM6PBEEHP62R39F9II0PBMCLP7IT38D5N6E891", MultibaseEncoding.Base32HexUpper)]
        [TestCase("et1sg3mqqt3gn5djxj11y3msci3817depfzgqejb", MultibaseEncoding.Base32Z)]
        [TestCase("RGVjZW50cmFsaXplIGV2ZXJ5dGhpbmchIQ==", MultibaseEncoding.Base64Padded)]
        public void TestTryDecoding_GivenValidEncodedInput_Unprefixed(string input, MultibaseEncoding encoding)
        {
            var expected = "Decentralize everything!!";
            var result = Multibase.TryDecode(input, out var decodedEncoding, out var decodedBytes);

            Assert.True(result);
            Assert.AreEqual(encoding, decodedEncoding);
            Assert.AreEqual(expected, Encoding.UTF8.GetString(decodedBytes));
        }

        // Official test vectors
        private static void TestVector(string encoding, string encoded, string expected)
        {
            var decoded = Multibase.Decode(encoded, out string mbEncoding);

            Assert.AreEqual(encoding, mbEncoding);
            Assert.AreEqual(expected, Encoding.UTF8.GetString(decoded));

            var rencoded = Multibase.Encode(mbEncoding, decoded);

            Assert.AreEqual(encoded, rencoded);
        }

        [Theory, TestCaseSource(nameof(GetCsvData),new object[] { "test1.csv" })]
        public void TestVector_1(string encoding, string encoded)
        {
            var expected = "Decentralize everything!!";

            TestVector(encoding, encoded, expected);
        }

        [Theory, TestCaseSource(nameof(GetCsvData), new object[] { "test2.csv" })]
        public void TestVector_2(string encoding, string encoded)
        {
            var expected = "yes mani !";

            TestVector(encoding, encoded, expected);
        }

        [Theory, TestCaseSource(nameof(GetCsvData), new object[] { "test3.csv" })]
        public void TestVector_3(string encoding, string encoded)
        {
            var expected = "hello world";

            TestVector(encoding, encoded, expected);
        }

        [Theory, TestCaseSource(nameof(GetCsvData), new object[] { "test4.csv" })]
        public void TestVector_4(string encoding, string encoded)
        {
            var expected = "\x00yes mani !";

            TestVector(encoding, encoded, expected);
        }

        [Theory, TestCaseSource(nameof(GetCsvData), new object[] { "test5.csv" })] 
        public void TestVector_5(string encoding, string encoded)
        {
            var expected = "\x00\x00yes mani !";

            TestVector(encoding, encoded, expected);
        }

        [Theory, TestCaseSource(nameof(GetCsvData), new object[] { "test6.csv" })]
        public void TestVector_6(string encoding, string encoded)
        {
            var expected = "hello world";

            var decoded = Multibase.Decode(encoded, out string mbEncoding, false);

            Assert.That(mbEncoding, Is.EqualTo(encoding));
            Assert.That(Encoding.UTF8.GetString(decoded), Is.EqualTo(expected));
        }

        [Test]
        public async Task TestBase58ConcurrentDecoding()
        {
            var tasks = Enumerable.Range(1, 10).Select(_ => Task.Run(() =>
            {
                var success = Multibase.TryDecode("Z6BLZQNPgws5ahFtr8x", out var encoding, out var bytes);
                Assert.True(success);
                Assert.AreEqual(MultibaseEncoding.Base58Flickr, encoding);
                Assert.AreEqual("Concurrency !", Encoding.UTF8.GetString(bytes, 0, bytes.Length));
            }));
            await Task.WhenAll(tasks);
        }

    private static IEnumerable<TestCaseData> GetCsvData(string filename)
    {
        foreach (var line in File.ReadLines(filename).Skip(1))
        {
            //csvFile.ReadLine();// Delimiter Row: "sep=,". Comment out if not used
            var row = line.Split(',').Select(c => c.Trim('"', ' ')).ToArray();
            yield return new TestCaseData(row);
        }
    }

    //private static object[] ConvertParameters(IReadOnlyList<object> values, IReadOnlyList<Type> parameterTypes)
    //{
    //    var result = new object[parameterTypes.Count];
    //    for (var idx = 0; idx < parameterTypes.Count; idx++)
    //    {
    //        result[idx] = ConvertParameter(values[idx], parameterTypes[idx]);
    //    }

    //    return result;
    //}

    //private static object ConvertParameter(object parameter, Type parameterType)
    //{
    //    return parameterType == typeof(int) ? Convert.ToInt32(parameter) : parameter;
    //}
    }

    //[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    //public class CsvDataAttribute : DataAttribute
    //{
    //    private readonly string _fileName;
    //    public CsvDataAttribute(string fileName)
    //    {
    //        _fileName = fileName;
    //    }

    //    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    //    {
    //        var pars = testMethod.GetParameters();
    //        var parameterTypes = pars.Select(par => par.ParameterType).ToArray();
    //        foreach (var line in File.ReadLines(_fileName).Skip(1))
    //        {
    //            //csvFile.ReadLine();// Delimiter Row: "sep=,". Comment out if not used
    //            var row = line.Split(',').Select(c => c.Trim('"', ' ')).ToArray();
    //            yield return ConvertParameters(row, parameterTypes);
    //        }
    //    }


    //}
}
