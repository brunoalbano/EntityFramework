// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Metadata.ModelConventions;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.ValueGeneration;

namespace Microsoft.Data.Entity.Storage
{
    public interface IDatabaseProviderServices
    {
        IDatabase Database { get; }
        IDatabaseCreator Creator { get; }
        IDatabaseConnection Connection { get; }
        IValueGeneratorSelector ValueGeneratorSelector { get; }
        IConventionSetBuilder ConventionSetBuilder { get; }
        IModelSource ModelSource { get; }
        IModelValidator ModelValidator { get; }
        IQueryContextFactory QueryContextFactory { get; }
        IValueGeneratorCache ValueGeneratorCache { get; }
    }
}
