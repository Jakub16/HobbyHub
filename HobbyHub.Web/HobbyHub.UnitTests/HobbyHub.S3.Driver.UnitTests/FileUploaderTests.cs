using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Serilog;

namespace HobbyHub.S3.Driver.UnitTests;

public class FileUploaderTests
{
    private Mock<ILogger> _loggerMock;
    private FileUploader _sut;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger>();
        _sut = new FileUploader(_loggerMock.Object);
    }

    [Test]
    public async Task SendShouldReturnUrlWhenUploadIsSuccessful()
    {
        var destination = "test-destination";
        var name = "test-name";
        var fileMock = new Mock<IFormFile>();
        var fileName = "test.jpg";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write("Test file content");
        writer.Flush();
        ms.Position = 0;
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(ms.Length);

        var result = await _sut.Send(destination, name, fileMock.Object);

        result.Should().StartWith("https://hobbyhub.s3.eu-north-1.amazonaws.com/");
    }
}