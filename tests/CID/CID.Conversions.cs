using System;
using Multiformats.CID;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Text.Encodings.Web;
using Multiformats.Hashes;
using System.Security.Cryptography;

namespace Multiformats.Tests;

public class CIDConversionsTests
{

    [Test(Description = "should convert v0 to v1")]
    public void ShouldConvertV0ToV1()
    {
        Digest digest = new SHA256Hasher().Digest($"TEST{DateTime.UtcNow}");
        var cid = CID.CID.Create(CIDVersion.CIDv0, 112, digest).ToV1();
        Assert.That(cid.Version, Is.EqualTo(CIDVersion.CIDv1));

    }

    [Test(Description = "should convert v1 to v0")]
    public void ShouldConvertV1ToV0()
    {
        Digest digest = new SHA256Hasher().Digest($"TEST{DateTime.UtcNow}");
        var cid = CID.CID.Create(CIDVersion.CIDv1, 112, digest).ToV0();
        Assert.That(cid.Version, Is.EqualTo(CIDVersion.CIDv0));
    }

    [Test(Description = "should not convert v1 to v0 if not dag-pb codec")]
    public void ShouldNotConvertV1ToV0IfNotDAGPBCodec()
    {
        Digest digest = new SHA256Hasher().Digest($"TEST{DateTime.UtcNow}");
        var cid = CID.CID.Create(CIDVersion.CIDv1, 0x71, digest);
        Assert.Throws<Exception>(() => cid.ToV0(), "Cannot convert a non dag-pb CID to CIDv0");
    }

    [Test(Description = "should not convert v1 to v0 if not sha2-256 multihash")]
    public void ShouldNotConvertV1ToV0IfNotSHA256Multihash()
    {
        var hash = new SHA512Hasher().Digest($"TEST{ DateTime.Now}");
        var cid = CID.CID.Create(1, 112, hash);
        Assert.Throws<Exception>(() => cid.ToV0(), "Cannot convert non sha2-256 multihash CID to CIDv0");
    }
}
