using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using Multiformats.Base;
using Multiformats.CID;
using Multiformats.Hashes;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Multiformats.Tests
{
    public class CIDv1
    {
        public CIDv1()
        {
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test(Description = "handles CID String (multibase encoded)")]
        public void HandlesCIDMultibaseEncoded()
        {
            string cidStr = "zdj7Wd8AMwqnhJGQCbFxBVodGSBG84TM7Hs1rcJuQMwTyfEDS";
            Multiformats.CID.CID cid = Multiformats.CID.CID.Parse(cidStr);
            Assert.That(cid.Version, Is.EqualTo(CIDVersion.CIDv1));
            Assert.That(cid.Code, Is.EqualTo(112));
            Assert.That(cid.ToString(), Is.EqualTo(new Base32Lower().Encode(cid.Bytes)));
        }

        [Test(Description = "handles CID (no multibase)")]
        public void HandlesCIDNoMultibase()
        {
            string cidStr = "bafybeidskjjd4zmr7oh6ku6wp72vvbxyibcli2r6if3ocdcy7jjjusvl2u";
            byte[] cidBuf = TestUtils.HexStringToByteArray("017012207252523e6591fb8fe553d67ff55a86f84044b46a3e4176e10c58fa529a4aabd5");
            var cid = CID.CID.Decode(cidBuf);
            Assert.That(cid.Code, Is.EqualTo(112));
            Assert.That(cid.Version, Is.EqualTo(CIDVersion.CIDv1));
            Assert.That(cid.ToString(), Is.EqualTo(cidStr));
        }

        [Test(Description = "create by parts")]
        public void CreateByParts()
        {
            Digest digest = new SHA256Hasher().Digest("abc");
            var cid = CID.CID.Create(CIDVersion.CIDv1, 0x71, digest);
            Assert.That(cid.Code, Is.EqualTo(0x71));
            Assert.That(cid.Version, Is.EqualTo(1));
            Assert.That(cid.Multihash, Is.EqualTo(digest));
        }

        [Test(Description = "CID.CreateV1")]
        public void CreateV1()
        {
            Digest digest = new SHA256Hasher().Digest("abc");
            var cid = CID.CID.CreateV1(0x71, digest);
            Assert.That(cid.Code, Is.EqualTo(0x71));
            Assert.That(cid.Version, Is.EqualTo(1));
            Assert.That(cid.Multihash, Is.EqualTo(digest));
        }

        [Test(Description = "can roundtrip through cid.toString()")]
        public void CanRoundtripThroughCIDToString()
        {
            Digest digest = new SHA256Hasher().Digest("abc");
            var cid1 = CID.CID.Create(CIDVersion.CIDv1, 0x71, digest);
            var cid1String = cid1.ToString();
            var cid2 = CID.CID.Parse(cid1String);

            Assert.That(cid1.Code, Is.EqualTo(cid2.Code));
            Assert.That(cid1.Version, Is.EqualTo(cid2.Version));
            Assert.That(cid1.Multihash, Is.EqualTo(cid2.Multihash));
        }

        [Test(Description = ".bytes")]
        public void ValidateBytes()
        {
            Digest digest = new SHA256Hasher().Digest("abc");
            var cid1 = CID.CID.Create(CIDVersion.CIDv1, 0x71, digest);
            var cidBytes = cid1.Bytes;
            var hexCID = BitConverter.ToString(cidBytes).Replace("-", "").ToLower();

            Assert.That(hexCID, Is.EqualTo("01711220ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad"));
        }

    }
}

