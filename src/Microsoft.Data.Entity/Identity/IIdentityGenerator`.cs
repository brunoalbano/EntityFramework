﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.Identity
{
    public interface IIdentityGenerator<T> : IIdentityGenerator
    {
        new Task<T> NextAsync(CancellationToken cancellationToken);
    }
}