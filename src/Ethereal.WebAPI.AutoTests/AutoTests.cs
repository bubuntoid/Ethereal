using System;
using System.Threading.Tasks;
using Ethereal.WebAPI.Client;
using Ethereal.WebAPI.Contracts;
using FluentAssertions;
using Flurl.Http;
using NUnit.Framework;

namespace Ethereal.WebAPI.AutoTests
{
    [TestFixture]
    public class AutoTests
    {
        private EtherealClient client;
        
        [SetUp]
        public void SetUp()
        {
            client = new EtherealClient(new EtherealClientSettings());
        }

        [Test]
        public async Task Initialize_CorrectUrl_JobIdReturned()
        {
            var response = await client.InitializeAsync(new InitializeJobRequestDto
            {
                Url = "https://www.youtube.com/watch?v=TmQyfUpyeFk&ab_channel=NewRetroWave"
            });

            response.Id.Should().NotBe(Guid.Empty);
        }
        
        [Test]
        public void Initialize_InvalidUrl_ErrorExpected()
        {
            Assert.ThrowsAsync<FlurlHttpException>(() => client.InitializeAsync(new InitializeJobRequestDto
            {
                Url = "https://www.youzxctube.com/watch?v=TmQyfUb_channel=NewRetroWave"
            }));
        }

        [Test]
        public async Task Get_ProcessingJobReturned()
        {
            var response = await client.InitializeAsync(new InitializeJobRequestDto
            {
                Url = "https://www.youtube.com/watch?v=TmQyfUpyeFk&ab_channel=NewRetroWave"
            });

            var job = await client.GetAsync(response.Id);

            job.Should().NotBeNull();
        }
    }
}