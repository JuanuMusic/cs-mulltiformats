using Multiformats.Base;
using Multiformats.CID;
using Multiformats.Hashes;
using NUnit.Framework;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace Multiformats.Tests;

public class CIDv0Tests
{

    [SetUp]
    public void Setup()
    {
    }

    [Test(Description = "handles B58Str multihash")]
    public void HandlesB58StrMultihash()
    {
        string mhStr = "QmdfTbBqBPQ7VNxZEYEj14VmRuZBkqFbiwReogJgS1zR1n";
        string expectedHex = "1220e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        CID.CID cid =  CID.CID.Parse(mhStr);

        Assert.That(cid.Version, Is.EqualTo(CIDVersion.CIDv0));
        Assert.That(cid.Code, Is.EqualTo(112));
        Assert.That(cid.Multihash.Bytes, Is.EqualTo(new Base58Btc().Decode(mhStr)));
    }

    [Test(Description = "create by parts")]
    public void CreateByParts()
    {
        Digest digest = new SHA256Hasher().Digest("abc");
        CID.CID cid = CID.CID.Create(CIDVersion.CIDv0, 112, digest);

        Assert.That(cid.Code, Is.EqualTo(112));
        Assert.That(cid.Version, Is.EqualTo(CIDVersion.CIDv0));
        Assert.That(cid.Multihash.Bytes, Is.EqualTo(digest.Bytes));
        Assert.That(cid.ToString(), Is.EqualTo(new Base58Btc().BaseEncode(digest.Bytes)));
    }

    [Test(Description = "CID.createV0")]
    public void CreateV0()
    {
        Digest digest = new SHA256Hasher().Digest("abc");
        CID.CID cid = CID.CID.CreateV0(digest);

        Assert.That(cid.Code, Is.EqualTo(112));
        Assert.That(cid.Version, Is.EqualTo(CIDVersion.CIDv0));
        Assert.That(cid.Multihash.Bytes, Is.EqualTo(digest.Bytes));
        Assert.That(cid.ToString(), Is.EqualTo(new Base58Btc().BaseEncode(digest.Bytes)));
    }

    [Test(Description = "create from multihash")]
    public void CreateFromMultihash()
    {
        Digest digest = new SHA256Hasher().Digest("abc");
        CID.CID cid = CID.CID.Decode(digest.Bytes);

        Assert.That(cid.Code, Is.EqualTo(112));
        Assert.That(cid.Version, Is.EqualTo(CIDVersion.CIDv0));
        Assert.That(cid.Multihash.Bytes, Is.EqualTo(digest.Bytes));
        Assert.That(cid.ToString(), Is.EqualTo(new Base58Btc().BaseEncode(digest.Bytes)));
    }

    [Test(Description = "throws on invalid BS58Str multihash")]
    public void ThrowsOnInvalidBase58StrMultihash()
    {
        Assert.Throws<InvalidDataException>(() => CID.CID.Parse("QmdfTbBqBPQ7VNxZEYEj14VmRuZBkqFbiwReogJgS1zIII"));
    }

    [Test(Description = "throws on trying to base encode CIDv0 in other base than dag-pb")]
    public void ThrowsOnTryingToBaseEncodeCIDV0InOtherBaseThanDAGPB()
    {
        Digest digest = new SHA256Hasher().Digest("abc");
        Assert.Throws<InvalidDataException>(() => CID.CID.Create(0, 113, digest), "Version 0 CID must use dag-pb (code: 112) block encoding");
    }

    [Test(Description = "outputs correct hex data")]
    public void GeneratesCorrectHexData()
    {
        Digest digest = new SHA256Hasher().Digest("abc");
        CID.CID cid = CID.CID.Create(CIDVersion.CIDv0, 112, digest);
        string hexCid = BitConverter.ToString(cid.Bytes).Replace("-","").ToLower();
        Assert.That(hexCid, Is.EqualTo("1220ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad"));
    }

    [Test(Description = "inspect bytes")]
    public void InspectBytes()
    {
        byte[] byts = Helpers.Conversions.HexStringToByteArray("1220ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad");
        
        CIDSpecs specs = CID.CID.InspectBytes(byts); // should only need the first few bytes
        Assert.That(specs, Is.EqualTo(new CIDSpecs {
            Version = 0,
            Codec = 0x70,
            MultihashCode = 0x12,
            MultihashSize = 34,
            DigestSize = 32,
            Size = 34
        }));
    }

    [Test(Description = "decodeFirst - no remainder")]
    public void NoRemainder()
    {
        byte[] byts = Helpers.Conversions.HexStringToByteArray("1220ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad");

        (CID.CID cid, byte[] remainder) = CID.CID.DecodeFirst(byts);
        
        Assert.That(cid.ToString(), Is.EqualTo("QmatYkNGZnELf8cAGdyJpUca2PyY4szai3RHyyWofNY1pY"));
        Assert.That(remainder.Length, Is.EqualTo(0));
    }

    [Test(Description = "decodeFirst - remainder")]
    public void Remainder()
    {
        byte[] byts = Helpers.Conversions.HexStringToByteArray("1220ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad0102030405");

        (CID.CID cid, byte[] remainder) = CID.CID.DecodeFirst(byts);

        Assert.That(cid.ToString(), Is.EqualTo("QmatYkNGZnELf8cAGdyJpUca2PyY4szai3RHyyWofNY1pY"));
        Assert.That(BitConverter.ToString(remainder).Replace("-",""), Is.EqualTo("0102030405"));
    }
}
