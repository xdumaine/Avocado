﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WinRTXamlToolkit.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Provides information about margins necessary for values.
    /// </summary>
    public interface IValueMarginProvider
    {
        /// <summary>
        /// Gets the margins required for values.
        /// </summary>
        /// <param name="consumer">The axis to retrieve the value margins 
        /// for.</param>
        /// <returns>The margins required for values.</returns>
        IEnumerable<ValueMargin> GetValueMargins(IValueMarginConsumer consumer);
    }
}
