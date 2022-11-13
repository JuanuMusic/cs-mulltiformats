using System;
using Multiformats.CID;
using NUnit.Framework;

namespace Multiformats.Tests
{
    public class CIDUtilitiesTests
    {
        string h1 = "QmdfTbBqBPQ7VNxZEYEj14VmRuZBkqFbiwReogJgS1zR1n";
        string h2 = "QmdfTbBqBPQ7VNxZEYEj14VmRuZBkqFbiwReogJgS1zR1o";
        string h3 = "zdj7Wd8AMwqnhJGQCbFxBVodGSBG84TM7Hs1rcJuQMwTyfEDS";

        [Test(Description = "V0 equals to V0")]
        public void V0EqualsV0()
        {
            var cid1 = CID.CID.Parse(h1);
            Assert.That(cid1, Is.EqualTo(CID.CID.Parse(h1)));
            Assert.That(cid1, Is.EqualTo(CID.CID.Create(cid1.Version, cid1.Code, cid1.Multihash)));

            var cid2 = CID.CID.Parse(h2);
            Assert.That(cid1, Is.Not.EqualTo(CID.CID.Parse(h2)));
            Assert.That(cid1, Is.Not.EqualTo(CID.CID.Create(cid2.Version, cid2.Code, cid2.Multihash)));

        }

        [Test]
        public void V0NotEqualsV1()
        {
            var cidV1 = CID.CID.Parse(h3);
            CID.CID cidV0 = cidV1.ToV0();

            Assert.That(cidV0, Is.Not.EqualTo(cidV1));
            Assert.That(cidV1, Is.Not.EqualTo(cidV0));

            Assert.That(cidV1.Multihash, Is.EqualTo(cidV0.Multihash));
        }

        [Test]
        public void V1EqualsV1()
        {
            var cid1 = CID.CID.Parse(h3);
            Assert.That(CID.CID.Parse(h3), Is.EqualTo(cid1));
            Assert.That(CID.CID.Create(cid1.Version, cid1.Code, cid1.Multihash), Is.EqualTo(cid1));
        }


    }
}

