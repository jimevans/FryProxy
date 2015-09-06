using OpenQA.Selenium;

namespace FryProxy.Tests.Integration {

    public class ChromeIntegratonTests : CommonIntegrationTests {

        protected override IWebDriver CreateDriver(Proxy proxy) {
            return CreateChromeDriver(proxy);
        }

    }

}