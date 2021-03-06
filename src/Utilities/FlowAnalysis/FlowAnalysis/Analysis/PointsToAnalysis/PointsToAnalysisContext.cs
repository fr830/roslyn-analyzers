﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Analyzer.Utilities.PooledObjects;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;

#pragma warning disable CA1067 // Override Object.Equals(object) when implementing IEquatable<T>

namespace Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.PointsToAnalysis
{
    using InterproceduralPointsToAnalysisData = InterproceduralAnalysisData<PointsToAnalysisData, PointsToAnalysisContext, PointsToAbstractValue>;
    using CopyAnalysisResult = DataFlowAnalysisResult<CopyBlockAnalysisResult, CopyAbstractValue>;

    /// <summary>
    /// Analysis context for execution of <see cref="PointsToAnalysis"/> on a control flow graph.
    /// </summary>
    public sealed class PointsToAnalysisContext : AbstractDataFlowAnalysisContext<PointsToAnalysisData, PointsToAnalysisContext, PointsToAnalysisResult, PointsToAbstractValue>
    {
        private PointsToAnalysisContext(
            AbstractValueDomain<PointsToAbstractValue> valueDomain,
            WellKnownTypeProvider wellKnownTypeProvider,
            ControlFlowGraph controlFlowGraph,
            ISymbol owningSymbol,
            InterproceduralAnalysisConfiguration interproceduralAnalysisConfig,
            bool pessimisticAnalysis,
            bool exceptionPathsAnalysis,
            CopyAnalysisResult copyAnalysisResultOpt,
            Func<PointsToAnalysisContext, PointsToAnalysisResult> getOrComputeAnalysisResult,
            ControlFlowGraph parentControlFlowGraphOpt,
            InterproceduralPointsToAnalysisData interproceduralAnalysisDataOpt,
            InterproceduralAnalysisPredicate interproceduralAnalysisPredicateOpt)
            : base(valueDomain, wellKnownTypeProvider, controlFlowGraph, owningSymbol, interproceduralAnalysisConfig, pessimisticAnalysis,
                  predicateAnalysis: true, exceptionPathsAnalysis, copyAnalysisResultOpt, pointsToAnalysisResultOpt: null,
                  getOrComputeAnalysisResult, parentControlFlowGraphOpt, interproceduralAnalysisDataOpt, interproceduralAnalysisPredicateOpt)
        {
        }

        internal static PointsToAnalysisContext Create(
            AbstractValueDomain<PointsToAbstractValue> valueDomain,
            WellKnownTypeProvider wellKnownTypeProvider,
            ControlFlowGraph controlFlowGraph,
            ISymbol owningSymbol,
            InterproceduralAnalysisConfiguration interproceduralAnalysisConfig,
            bool pessimisticAnalysis,
            bool exceptionPathsAnalysis,
            CopyAnalysisResult copyAnalysisResultOpt,
            Func<PointsToAnalysisContext, PointsToAnalysisResult> getOrComputeAnalysisResult,
            InterproceduralAnalysisPredicate interproceduralAnalysisPredicateOpt)
        {
            return new PointsToAnalysisContext(valueDomain, wellKnownTypeProvider, controlFlowGraph, owningSymbol, interproceduralAnalysisConfig,
                pessimisticAnalysis, exceptionPathsAnalysis, copyAnalysisResultOpt, getOrComputeAnalysisResult, parentControlFlowGraphOpt: null,
                interproceduralAnalysisDataOpt: null, interproceduralAnalysisPredicateOpt);
        }

        public override PointsToAnalysisContext ForkForInterproceduralAnalysis(
            IMethodSymbol invokedMethod,
            ControlFlowGraph invokedControlFlowGraph,
            IOperation operation,
            PointsToAnalysisResult pointsToAnalysisResultOpt,
            CopyAnalysisResult copyAnalysisResultOpt,
            InterproceduralPointsToAnalysisData interproceduralAnalysisData)
        {
            Debug.Assert(pointsToAnalysisResultOpt == null);

            return new PointsToAnalysisContext(ValueDomain, WellKnownTypeProvider, invokedControlFlowGraph, invokedMethod, InterproceduralAnalysisConfiguration,
                PessimisticAnalysis, ExceptionPathsAnalysis, copyAnalysisResultOpt, GetOrComputeAnalysisResult, ControlFlowGraph, interproceduralAnalysisData,
                InterproceduralAnalysisPredicateOpt);
        }

        protected override void ComputeHashCodePartsSpecific(ArrayBuilder<int> builder)
        {
        }
    }
}
