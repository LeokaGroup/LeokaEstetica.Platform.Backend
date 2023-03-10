using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.UserTests;

[TestFixture]
public class SignInTest : BaseServiceTest
{
    [Test]
    public async Task SignInAsyncTest()
    {
        var result = await UserService.SignInAsync("sierra_93@mail.ru", "12345");
        
        Assert.IsNotNull(result);
        Assert.IsTrue(!result.Errors.Any());
        Assert.IsNotNull(result.Token);
    }

    [Test]
    public async Task SignInGoogleAsyncTest()
    {
        var testToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjVkZjFmOTQ1ZmY5MDZhZWFlZmE5M2MyNzY5OGRiNDA2ZDYwNmIwZTgiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJuYmYiOjE2NzgzOTU5OTQsImF1ZCI6IjQxODk5OTk1MTg3NS1zMXNtdHY4b2l0bjU3OWk4cGQ0bmEwNTlwbmJjdGYxOS5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsInN1YiI6IjExNzY3MjA3ODM0MDg3NTI2MDc3MyIsImVtYWlsIjoib3ZlcnRlY2hjb25zdWx0aW5nQGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJhenAiOiI0MTg5OTk5NTE4NzUtczFzbXR2OG9pdG41NzlpOHBkNG5hMDU5cG5iY3RmMTkuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJuYW1lIjoi0JDQvdGC0L7QvSDQotC40LzQvtGI0LXQvdC60L4iLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EvQUdObXl4YWVQVXpBZmxKU3UzRU1nU2drbkd4QzRTSkZEOTBqVWk2dWlYZ2hFdz1zOTYtYyIsImdpdmVuX25hbWUiOiLQkNC90YLQvtC9IiwiZmFtaWx5X25hbWUiOiLQotC40LzQvtGI0LXQvdC60L4iLCJpYXQiOjE2NzgzOTYyOTQsImV4cCI6MTY3ODM5OTg5NCwianRpIjoiYzE2Yjk3OTRjM2M1OWUzMzg2M2JjOWNmZDk3Y2VjZWYxNzYwNGQxMCJ9.iMZJlY6u5Wm4HKql_waYyDegk7SopgsyGZX6oUqqoNdCbpGEGYIIeNg4zF-dQI9BKkgljDeKll4m3GONW80SH-UN9xDQWVnpB4Ga_L_851I82MNR7HGu4soHvVewMFBzRIpDL1dL7pKQVYEcfCuwpYojqw8NkXSFt7u4VJ8GkKosNjujQ9r1avcQMrUFxbSOVL_SoF0ND2aolcZoTZTK-kRBCHdEx-7g1m-wiGd-w7S_ASBe15-PBMJjoP70tmU9eLzs_VpajHlSBW0S3I_LgO0sgch4k0w0GmhkKZ0IyE2xwDTcApAOOw5wVW0Et61QMMcEfiv3WwMz6-J3bXJ5tw";

        var result = await UserService.SignInAsync(testToken);

        Assert.NotNull(result);
    }
}