// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Storage;
using Moq;
using Xunit;

namespace Microsoft.Data.Entity.Tests.Storage
{
    public class DatabaseProviderSelectorTest
    {
        [Fact]
        public void Selects_single_configured_store()
        {
            var services = Mock.Of<IDatabaseProviderServices>();
            var source = CreateSource("DataStore1", configured: true, available: false, services: services);

            var selector = new DatabaseProviderSelector(Mock.Of<IServiceProvider>(), Mock.Of<IDbContextOptions>(), new[] { source });

            Assert.Same(services, selector.SelectServices(ServiceProviderSource.Explicit));
        }

        [Fact]
        public void Throws_if_multiple_stores_configured()
        {
            var source1 = CreateSource("DataStore1", configured: true, available: false);
            var source2 = CreateSource("DataStore2", configured: true, available: false);
            var source3 = CreateSource("DataStore3", configured: false, available: true);
            var source4 = CreateSource("DataStore4", configured: true, available: false);

            var selector = new DatabaseProviderSelector(
                Mock.Of<IServiceProvider>(), 
                Mock.Of<IDbContextOptions>(), 
                new[] { source1, source2, source3, source4 });

            Assert.Equal(Strings.MultipleProvidersConfigured("'DataStore1' 'DataStore2' 'DataStore4' "),
                Assert.Throws<InvalidOperationException>(
                    () => selector.SelectServices(ServiceProviderSource.Explicit)).Message);
        }

        [Fact]
        public void Throws_if_no_store_services_have_been_registered_using_external_service_provider()
        {
            var selector = new DatabaseProviderSelector(
                Mock.Of<IServiceProvider>(),
                Mock.Of<IDbContextOptions>(),
                null);

            Assert.Equal(Strings.NoProviderServices,
                Assert.Throws<InvalidOperationException>(
                    () => selector.SelectServices(ServiceProviderSource.Explicit)).Message);
        }

        [Fact]
        public void Throws_if_no_store_services_have_been_registered_using_implicit_service_provider()
        {
            var selector = new DatabaseProviderSelector(
                Mock.Of<IServiceProvider>(),
                Mock.Of<IDbContextOptions>(),
                null);

            Assert.Equal(Strings.NoProviderConfigured,
                Assert.Throws<InvalidOperationException>(
                    () => selector.SelectServices(ServiceProviderSource.Implicit)).Message);
        }

        [Fact]
        public void Throws_if_multiple_store_services_are_registered_but_none_are_configured()
        {
            var source1 = CreateSource("DataStore1", configured: false, available: true);
            var source2 = CreateSource("DataStore2", configured: false, available: false);
            var source3 = CreateSource("DataStore3", configured: false, available: false);

            var selector = new DatabaseProviderSelector(
                Mock.Of<IServiceProvider>(),
                Mock.Of<IDbContextOptions>(),
                new[] { source1, source2, source3 });

            Assert.Equal(Strings.MultipleProvidersAvailable("'DataStore1' 'DataStore2' 'DataStore3' "),
                Assert.Throws<InvalidOperationException>(
                    () => selector.SelectServices(ServiceProviderSource.Explicit)).Message);
        }

        [Fact]
        public void Throws_if_one_store_service_is_registered_but_not_configured_and_cannot_be_used_without_configuration()
        {
            var source = CreateSource("DataStore1", configured: false, available: false);

            var selector = new DatabaseProviderSelector(
                Mock.Of<IServiceProvider>(),
                Mock.Of<IDbContextOptions>(),
                new[] { source });

            Assert.Equal(Strings.NoProviderConfigured,
                Assert.Throws<InvalidOperationException>(
                    () => selector.SelectServices(ServiceProviderSource.Explicit)).Message);
        }

        private static IDatabaseProvider CreateSource(string name, bool configured, bool available, IDatabaseProviderServices services = null)
        {
            var sourceMock = new Mock<IDatabaseProvider>();
            sourceMock.Setup(m => m.IsConfigured(It.IsAny<IDbContextOptions>())).Returns(configured);
            sourceMock.Setup(m => m.GetProviderServices(It.IsAny<IServiceProvider>())).Returns(services);
            sourceMock.Setup(m => m.Name).Returns(name);

            return sourceMock.Object;
        }
    }
}
