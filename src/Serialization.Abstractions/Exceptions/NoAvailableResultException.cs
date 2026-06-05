// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Linq.Expressions.Serialization.Exceptions;

/// <summary>
/// Thrown when there are no available results to be returned from a transform.
/// </summary>
/// <param name="message">The exception message.</param>
/// <param name="innerException">The inner exception.</param>
[ExcludeFromCodeCoverage]
public class NoAvailableResultException(string? message = null, Exception? innerException = null)
    : InvalidOperationException(message ?? "There are no available transform results. Did you call the visitor already?", innerException);
