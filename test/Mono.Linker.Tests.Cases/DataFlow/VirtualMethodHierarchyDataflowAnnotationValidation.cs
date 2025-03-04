﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;

namespace Mono.Linker.Tests.Cases.DataFlow
{
	[SkipKeptItemsValidation]
	[SandboxDependency ("Dependencies/TestSystemTypeBase.cs")]

	// Suppress warnings about accessing methods with annotations via reflection - the test below does that a LOT
	// (The test accessed these methods through DynamicallyAccessedMembers annotations which is effectively the same reflection access)
	[UnconditionalSuppressMessage ("test", "IL2111")]
	class VirtualMethodHierarchyDataflowAnnotationValidation
	{
		public static void Main ()
		{
			// The test uses data flow annotation to mark all public methods on the specified types
			// which in turn will trigger querying the annotations on those methods and thus the validation.

			RequirePublicMethods (typeof (BaseClass));
			RequirePublicMethods (typeof (DerivedClass));
			RequirePublicMethods (typeof (SuperDerivedClass));
			RequirePublicMethods (typeof (DerivedOverNoAnnotations));
			RequirePublicMethods (typeof (DerivedWithNoAnnotations));
			RequirePublicMethods (typeof (IBase));
			RequirePublicMethods (typeof (IDerived));
			RequirePublicMethods (typeof (ImplementationClass));
			RequirePublicMethods (typeof (IBaseImplementedInterface));
			RequirePublicMethods (typeof (BaseImplementsInterfaceViaDerived));
			RequirePublicMethods (typeof (DerivedWithInterfaceImplementedByBase));
			RequirePublicMethods (typeof (VirtualMethodHierarchyDataflowAnnotationValidationTypeTestBase));
			RequirePublicMethods (typeof (VirtualMethodHierarchyDataflowAnnotationValidationTypeTestDerived));
			RequirePublicMethods (typeof (ITwoInterfacesImplementedByOneMethod_One));
			RequirePublicMethods (typeof (ITwoInterfacesImplementedByOneMethod_Two));
			RequirePublicMethods (typeof (ImplementationOfTwoInterfacesWithOneMethod));
		}

		static void RequirePublicMethods ([DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)] Type type)
		{
		}

		class BaseClass
		{
			// === Return values ===
			// Other than the basics, the return value also checks all of the inheritance cases - we omit those for the other tests
			public virtual Type ReturnValueBaseWithoutDerivedWithout () => null;
			public virtual Type ReturnValueBaseWithoutDerivedWith () => null;
			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			public virtual Type ReturnValueBaseWithDerivedWithout () => null;
			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			public virtual Type ReturnValueBaseWithDerivedWith () => null;

			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			public virtual Type ReturnValueBaseWithSuperDerivedWithout () => null;

			// === Method parameters ===
			// This does not check complicated inheritance cases as that is already validated by the return values
			public virtual void SingleParameterBaseWithDerivedWithout (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
				Type p)
			{ }

			public virtual void SingleParameterBaseWithDerivedWith_ (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
				Type p)
			{ }

			public virtual void SingleParameterBaseWithoutDerivedWith_ (Type p) { }

			public virtual void SingleParameterBaseWithoutDerivedWithout (Type p) { }

			public virtual void SingleParameterBaseWithDerivedWithDifferent (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)]
				Type p)
			{ }

			public virtual void MultipleParametersBaseWithDerivedWithout (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)]
				Type p1BaseWithDerivedWithout,
				Type p2BaseWithoutDerivedWithout,
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)]
				Type p3BaseWithDerivedWithout)
			{ }

			public virtual void MultipleParametersBaseWithoutDerivedWith (
				Type p1BaseWithoutDerivedWith,
				Type p2BaseWithoutDerivedWithout,
				Type p3BaseWithoutDerivedWith)
			{ }

			public virtual void MultipleParametersBaseWithDerivedWithMatch (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)]
				Type p1BaseWithDerivedWith,
				Type p2BaseWithoutDerivedWithout,
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicFields)]
				Type p3BaseWithDerivedWith)
			{ }

			public virtual void MultipleParametersBaseWithDerivedWithMismatch (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)]
				Type p1BaseWithDerivedWithMismatch,
				Type p2BaseWithoutDerivedWith,
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicFields)]
				Type p3BaseWithDerivedWithMatch,
				Type p4NoAnnotations)
			{ }

			// === Generic methods ===
			public virtual void GenericBaseWithDerivedWithout<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors)] T> () { }
			public virtual void GenericBaseWithoutDerivedWith<T> () { }
			public virtual void GenericBaseWithDerivedWith_<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors)] T> () { }
			public virtual void GenericBaseWithoutDerivedWithout<T> () { }

			// === Properties ===
			[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)]
			public virtual Type PropertyBaseWithDerivedWithout { get; }
			[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)]
			public virtual Type PropertyBaseWithDerivedWith_ { get; set; }
			[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)]
			public virtual Type PropertyBaseWithDerivedOnGetterWith { get; }

			// === RequiresUnreferencedCode ===
			[RequiresUnreferencedCode ("")]
			public virtual void RequiresUnreferencedCodeBaseWithDerivedWithout () { }
			public virtual void RequiresUnreferencedCodeBaseWithoutDerivedWith_ () { }
			[RequiresUnreferencedCode ("")]
			public virtual void RequiresUnreferencedCodeBaseWithDerivedWith_ () { }
			public virtual void RequiresUnreferencedCodeBaseWithoutDerivedWithout () { }

			public virtual void RequiresUnreferencedCodeBaseWithoutSuperDerivedWith_ () { }
		}

		class DerivedClass : BaseClass
		{
			// === Return values ===
			[LogDoesNotContain ("DerivedClass.ReturnValueBaseWithoutDerivedWithout")]
			public override Type ReturnValueBaseWithoutDerivedWithout () => null;

			[ExpectedWarning ("IL2093", "BaseClass.ReturnValueBaseWithoutDerivedWith", "DerivedClass.ReturnValueBaseWithoutDerivedWith")]
			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			public override Type ReturnValueBaseWithoutDerivedWith () => null;

			[LogContains ("DerivedClass.ReturnValueBaseWithDerivedWithout")]
			public override Type ReturnValueBaseWithDerivedWithout () => null;

			[LogDoesNotContain ("DerivedClass.ReturnValueBaseWithDerivedWitht")]
			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			public override Type ReturnValueBaseWithDerivedWith () => null;


			// === Method parameters ===
			[ExpectedWarning ("IL2092",
				"p", "Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.DerivedClass.SingleParameterBaseWithDerivedWithout(Type)",
				"p", "Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.BaseClass.SingleParameterBaseWithDerivedWithout(Type)")]
			public override void SingleParameterBaseWithDerivedWithout (Type p) { }

			[LogDoesNotContain ("DerivedClass.SingleParameterBaseWithDerivedWith_")]
			public override void SingleParameterBaseWithDerivedWith_ (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
				Type p)
			{ }

			[LogContains (
				"'DynamicallyAccessedMemberTypes' in 'DynamicallyAccessedMembersAttribute' on the parameter 'p' of method 'Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.DerivedClass.SingleParameterBaseWithoutDerivedWith_(Type)' " +
				"don't match overridden parameter 'p' of method 'Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.BaseClass.SingleParameterBaseWithoutDerivedWith_(Type)'. " +
				"All overridden members must have the same 'DynamicallyAccessedMembersAttribute' usage.")]
			public override void SingleParameterBaseWithoutDerivedWith_ (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
				Type p)
			{ }

			[LogDoesNotContain ("DerivedClass.SingleParameterBaseWithoutDerivedWithout")]
			public override void SingleParameterBaseWithoutDerivedWithout (Type p) { }

			[LogContains (
				"'DynamicallyAccessedMemberTypes' in 'DynamicallyAccessedMembersAttribute' on the parameter 'p' of method 'Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.DerivedClass.SingleParameterBaseWithDerivedWithDifferent(Type)' " +
				"don't match overridden parameter 'p' of method 'Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.BaseClass.SingleParameterBaseWithDerivedWithDifferent(Type)'. " +
				"All overridden members must have the same 'DynamicallyAccessedMembersAttribute' usage.")]
			public override void SingleParameterBaseWithDerivedWithDifferent (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicFields)]
				Type p)
			{ }


			[LogContains (".*'p1BaseWithDerivedWithout'.*DerivedClass.*MultipleParametersBaseWithDerivedWithout.*", regexMatch: true)]
			[LogDoesNotContain (".*'p2BaseWithoutDerivedWithout'.*DerivedClass.*MultipleParametersBaseWithDerivedWithout.*", regexMatch: true)]
			[LogContains (".*'p3BaseWithDerivedWithout'.*DerivedClass.*MultipleParametersBaseWithDerivedWithout.*", regexMatch: true)]
			public override void MultipleParametersBaseWithDerivedWithout (
				Type p1BaseWithDerivedWithout,
				Type p2BaseWithoutDerivedWithout,
				Type p3BaseWithDerivedWithout)
			{ }

			[LogContains (".*'p1BaseWithoutDerivedWith'.*DerivedClass.*MultipleParametersBaseWithoutDerivedWith.*", regexMatch: true)]
			[LogDoesNotContain (".*'p2BaseWithoutDerivedWithout'.*DerivedClass.*MultipleParametersBaseWithoutDerivedWith.*", regexMatch: true)]
			[LogContains (".*'p3BaseWithoutDerivedWith'.*DerivedClass.*MultipleParametersBaseWithoutDerivedWith.*", regexMatch: true)]
			public override void MultipleParametersBaseWithoutDerivedWith (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicFields)]
				Type p1BaseWithoutDerivedWith,
				Type p2BaseWithoutDerivedWithout,
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicFields)]
				Type p3BaseWithoutDerivedWith)
			{ }

			[LogDoesNotContain ("DerivedClass.MultipleParametersBaseWithDerivedWithMatch")]
			public override void MultipleParametersBaseWithDerivedWithMatch (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)]
				Type p1BaseWithDerivedWith,
				Type p2BaseWithoutDerivedWithout,
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicFields)]
				Type p3BaseWithDerivedWith)
			{ }

			[LogContains (".*'p1BaseWithDerivedWithMismatch'.*DerivedClass.*MultipleParametersBaseWithDerivedWithMismatch.*", regexMatch: true)]
			[LogContains (".*'p2BaseWithoutDerivedWith'.*DerivedClass.*MultipleParametersBaseWithDerivedWithMismatch.*", regexMatch: true)]
			[LogDoesNotContain (".*'p3BaseWithDerivedWithMatch'.*DerivedClass.*MultipleParametersBaseWithDerivedWithMismatch.*", regexMatch: true)]
			[LogDoesNotContain (".*'p4NoAnnotations'.*DerivedClass.*MultipleParametersBaseWithDerivedWithMismatch.*", regexMatch: true)]
			public override void MultipleParametersBaseWithDerivedWithMismatch (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicFields)]
				Type p1BaseWithDerivedWithMismatch,
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicFields)]
				Type p2BaseWithoutDerivedWith,
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicFields)]
				Type p3BaseWithDerivedWithMatch,
				Type p4NoAnnotations)
			{ }

			// === Generic methods ===
			[ExpectedWarning ("IL2095",
				"T", "Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.DerivedClass.GenericBaseWithDerivedWithout<T>()",
				"T", "Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.BaseClass.GenericBaseWithDerivedWithout<T>()")]
			public override void GenericBaseWithDerivedWithout<T> () { }

			[LogContains (
				"'DynamicallyAccessedMemberTypes' in 'DynamicallyAccessedMembersAttribute' on the generic parameter 'T' of 'Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.DerivedClass.GenericBaseWithoutDerivedWith<T>()' " +
				"don't match overridden generic parameter 'T' of 'Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.BaseClass.GenericBaseWithoutDerivedWith<T>()'. " +
				"All overridden members must have the same 'DynamicallyAccessedMembersAttribute' usage.")]
			public override void GenericBaseWithoutDerivedWith<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors)]T> () { }

			[LogDoesNotContain ("DerivedClass.GenericBaseWithDerivedWith_")]
			public override void GenericBaseWithDerivedWith_<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors)] T> () { }

			[LogDoesNotContain ("DerivedClass.GenericBaseWithoutDerivedWithout")]
			public override void GenericBaseWithoutDerivedWithout<T> () { }


			// === Properties ===
			// The warning is reported on the getter (or setter), which is not ideal, but it's probably good enough for now (we don't internally track annotations
			// on properties themselves, only on methods).
			[LogContains (
				"'DynamicallyAccessedMemberTypes' in 'DynamicallyAccessedMembersAttribute' on the return value of method 'Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.DerivedClass.PropertyBaseWithDerivedWithout.get' " +
				"don't match overridden return value of method 'Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.BaseClass.PropertyBaseWithDerivedWithout.get'. " +
				"All overridden members must have the same 'DynamicallyAccessedMembersAttribute' usage.")]
			public override Type PropertyBaseWithDerivedWithout { get; }

			[LogDoesNotContain ("DerivedClass.PropertyBaseWithDerivedWith_.get")]
			[LogDoesNotContain ("DerivedClass.PropertyBaseWithDerivedWith_.set")]
			[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)]
			public override Type PropertyBaseWithDerivedWith_ { get; set; }

			[LogDoesNotContain ("PropertyBaseWithDerivedOnGetterWith")]
			[field: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)]
			public override Type PropertyBaseWithDerivedOnGetterWith { [return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)] get; }


			// === RequiresUnreferencedCode ===
			[ExpectedWarning ("IL2046", "DerivedClass.RequiresUnreferencedCodeBaseWithDerivedWithout()",
				"BaseClass.RequiresUnreferencedCodeBaseWithDerivedWithout()",
				"'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides")]
			public override void RequiresUnreferencedCodeBaseWithDerivedWithout () { }
			[ExpectedWarning ("IL2046", "DerivedClass.RequiresUnreferencedCodeBaseWithoutDerivedWith_()",
				"BaseClass.RequiresUnreferencedCodeBaseWithoutDerivedWith_()",
				"'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides")]
			[RequiresUnreferencedCode ("")]
			public override void RequiresUnreferencedCodeBaseWithoutDerivedWith_ () { }
			[LogDoesNotContain ("DerivedClass.RequiresUnreferencedCodeBaseWithDerivedWith_")]
			[RequiresUnreferencedCode ("")]
			public override void RequiresUnreferencedCodeBaseWithDerivedWith_ () { }
			[LogDoesNotContain ("DerivedClass.RequiresUnreferencedCodeBaseWithoutDerivedWithout")]
			public override void RequiresUnreferencedCodeBaseWithoutDerivedWithout () { }
		}

		class InBetweenDerived : DerivedClass
		{
			// This is intentionally left empty to validate that the logic can skip over to deeper base classes correctly
		}

		class SuperDerivedClass : InBetweenDerived
		{
			// === Return values ===
			[LogContains (
				"'DynamicallyAccessedMemberTypes' in 'DynamicallyAccessedMembersAttribute' on the return value of method 'Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.SuperDerivedClass.ReturnValueBaseWithSuperDerivedWithout()' " +
				"don't match overridden return value of method 'Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.BaseClass.ReturnValueBaseWithSuperDerivedWithout()'. " +
				"All overridden members must have the same 'DynamicallyAccessedMembersAttribute' usage.")]
			public override Type ReturnValueBaseWithSuperDerivedWithout () => null;

			// === RequiresUnreferencedCode ===
			[LogContains ("SuperDerivedClass.RequiresUnreferencedCodeBaseWithoutSuperDerivedWith_")]
			[RequiresUnreferencedCode ("")]
			public override void RequiresUnreferencedCodeBaseWithoutSuperDerivedWith_ () { }
		}


		abstract class BaseWithNoAnnotations
		{
			// This class must not have ANY annotations anywhere on it.
			// It's here to test that the optimization works (as most classes won't have any annotations, so we shortcut that path).

			// === Return values ===
			public abstract Type ReturnValueBaseWithoutDerivedWith ();

			public abstract Type ReturnValueBaseWithoutDerivedWithout ();

			// === Method parameters ===
			public virtual void SingleParameterBaseWithoutDerivedWith_ (Type p) { }

			public virtual void SingleParameterBaseWithoutDerivedWithout (Type p) { }

			// === Generic methods ===
			public virtual void GenericBaseWithoutDerivedWith_<T> () { }

			public virtual void GenericBaseWithoutDerivedWithout<T> () { }

			// === RequiresUnreferencedCode ===
			public virtual void RequiresUnreferencedCodeBaseWithoutDerivedWith_ () { }
			public virtual void RequiresUnreferencedCodeBaseWithoutDerivedWithout () { }
		}

		class DerivedOverNoAnnotations : BaseWithNoAnnotations
		{
			// === Return values ===
			[LogContains ("DerivedOverNoAnnotations.ReturnValueBaseWithoutDerivedWith")]
			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			public override Type ReturnValueBaseWithoutDerivedWith () => null;

			[LogDoesNotContain ("DerivedOverNoAnnotations.ReturnValueBaseWithoutDerivedWithout")]
			public override Type ReturnValueBaseWithoutDerivedWithout () => null;

			// === Method parameters ===
			[LogContains ("DerivedOverNoAnnotations.SingleParameterBaseWithoutDerivedWith_")]
			public override void SingleParameterBaseWithoutDerivedWith_ (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
				Type p)
			{ }

			[LogDoesNotContain ("DerivedOverNoAnnotations.SingleParameterBaseWithoutDerivedWithout")]
			public override void SingleParameterBaseWithoutDerivedWithout (Type p) { }

			// === Generic methods ===
			[LogContains ("DerivedOverNoAnnotations.GenericBaseWithoutDerivedWith_")]
			public override void GenericBaseWithoutDerivedWith_<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors)] T> () { }

			[LogDoesNotContain ("DerivedOverNoAnnotations.GenericBaseWithoutDerivedWithout")]
			public override void GenericBaseWithoutDerivedWithout<T> () { }


			// === RequiresUnreferencedCode ===
			[LogContains ("DerivedOverNoAnnotations.RequiresUnreferencedCodeBaseWithoutDerivedWith_")]
			[RequiresUnreferencedCode ("")]
			public override void RequiresUnreferencedCodeBaseWithoutDerivedWith_ () { }
			[LogDoesNotContain ("DerivedOverNoAnnotations.RequiresUnreferencedCodeBaseWithoutDerivedWithout")]
			public override void RequiresUnreferencedCodeBaseWithoutDerivedWithout () { }
		}


		abstract class BaseWithAnnotations
		{
			// === Return values ===
			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			public abstract Type ReturnValueBaseWithDerivedWithout ();

			public abstract Type ReturnValueBaseWithoutDerivedWithout ();

			// === Method parameters ===
			public virtual void SingleParameterBaseWithDerivedWithout (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
				Type p)
			{ }

			public virtual void SingleParameterBaseWithDerivedWith_ (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
				Type p)
			{ }

			// === Generic methods ===
			public virtual void GenericBaseWithDerivedWithout<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors)] T> () { }

			public virtual void GenericBaseWithoutDerivedWithout<T> () { }


			// === RequiresUnreferencedCode ===
			[RequiresUnreferencedCode ("")]
			public virtual void RequiresUnreferencedCodeBaseWithDerivedWith_ () { }
			[RequiresUnreferencedCode ("")]
			public virtual void RequiresUnreferencedCodeBaseWithDerivedWithout () { }
		}

		class DerivedWithNoAnnotations : BaseWithAnnotations
		{
			// This class must not have ANY annotations anywhere on it.
			// It's here to test that the optimization works (as most classes won't have any annotations, so we shortcut that path).

			// === Return values ===
			[LogContains ("DerivedWithNoAnnotations.ReturnValueBaseWithDerivedWithout")]
			public override Type ReturnValueBaseWithDerivedWithout () => null;

			[LogDoesNotContain ("DerivedWithNoAnnotations.ReturnValueBaseWithoutDerivedWithout")]
			public override Type ReturnValueBaseWithoutDerivedWithout () => null;

			// === Method parameters ===
			[LogContains ("DerivedWithNoAnnotations.SingleParameterBaseWithDerivedWithout")]
			public override void SingleParameterBaseWithDerivedWithout (Type p) { }

			[LogDoesNotContain ("DerivedWithNoAnnotations.SingleParameterBaseWithDerivedWith_")]
			public override void SingleParameterBaseWithDerivedWith_ (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
				Type p)
			{ }

			// === Generic methods ===
			[LogContains ("DerivedWithNoAnnotations.GenericBaseWithDerivedWithout")]
			public override void GenericBaseWithDerivedWithout<T> () { }

			[LogDoesNotContain ("DerivedWithNoAnnotations.GenericBaseWithoutDerivedWithout")]
			public override void GenericBaseWithoutDerivedWithout<T> () { }


			// === RequiresUnreferencedCode ===
			[LogDoesNotContain ("DerivedWithNoAnnotations.RequiresUnreferencedCodeBaseWithDerivedWith_")]
			[RequiresUnreferencedCode ("")]
			public override void RequiresUnreferencedCodeBaseWithDerivedWith_ () { }
			[LogContains ("DerivedWithNoAnnotations.RequiresUnreferencedCodeBaseWithDerivedWithout")]
			public override void RequiresUnreferencedCodeBaseWithDerivedWithout () { }
		}


		interface IBase
		{
			// === Return values ===
			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			Type ReturnValueInterfaceBaseWithImplementationWithout ();

			Type ReturnValueInterfaceBaseWithoutImplementationWith_ ();

			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			Type ReturnValueInterfaceBaseWithImplementationWith_ ();


			// === Method parameters ===
			void SingleParameterBaseWithImplementationWith_ (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
				Type p);

			void SingleParameterBaseWithoutImplementationWith_ (Type p);

			void SingleParameterBaseWithImplementationWithout (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
				Type p);

			void SingleParameterBaseWithoutImplementationWithout (Type p);


			// === Generic methods ===
			void GenericInterfaceBaseWithoutImplementationWith_<T> ();

			void GenericInterfaceBaseWithImplementationWithout<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors)] T> ();

			// === Properties ===
			Type PropertyInterfaceBaseWithoutImplementationWith { get; set; }


			// === RequiresUnreferencedCode ===
			[RequiresUnreferencedCode ("")]
			void RequiresUnreferencedCodeInterfaceBaseWithImplementationWith_ ();
			void RequiresUnreferencedCodeInterfaceBaseWithoutImplementationWith_ ();
		}

		interface IDerived : IBase
		{
			// === Return values ===
			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			Type ReturnTypeInterfaceDerivedWithImplementationWithout ();


			// === Method parameters ===
		}

		abstract class ImplementationClass : IDerived
		{
			// === Return values ===
			[LogContains ("ImplementationClass.ReturnValueInterfaceBaseWithImplementationWithout")]
			public Type ReturnValueInterfaceBaseWithImplementationWithout () => null;

			[LogContains ("ImplementationClass.ReturnValueInterfaceBaseWithoutImplementationWith_")]
			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			public Type ReturnValueInterfaceBaseWithoutImplementationWith_ () => null;

			[LogDoesNotContain ("ImplementationClass.ReturnValueInterfaceBaseWithImplementationWith_")]
			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			public Type ReturnValueInterfaceBaseWithImplementationWith_ () => null;

			[LogContains ("ImplementationClass.ReturnTypeInterfaceDerivedWithImplementationWithout")]
			public Type ReturnTypeInterfaceDerivedWithImplementationWithout () => null;


			// === Method parameters ===
			[LogDoesNotContain ("ImplementationClass.SingleParameterBaseWithImplementationWith_")]
			public void SingleParameterBaseWithImplementationWith_ (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
				Type p)
			{ }

			[LogContains ("ImplementationClass.SingleParameterBaseWithImplementationWithout")]
			public void SingleParameterBaseWithImplementationWithout (Type p) { }

			[LogContains ("ImplementationClass.SingleParameterBaseWithoutImplementationWith_")]
			public void SingleParameterBaseWithoutImplementationWith_ (
				[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
				Type p)
			{ }

			[LogDoesNotContain ("ImplementationClass.SingleParameterBaseWithoutImplementationWithout")]
			public void SingleParameterBaseWithoutImplementationWithout (Type p) { }


			// === Generic methods ===
			[LogContains ("ImplementationClass.GenericInterfaceBaseWithoutImplementationWith_")]
			public void GenericInterfaceBaseWithoutImplementationWith_<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors)] T> () { }

			[LogContains ("ImplementationClass.GenericInterfaceBaseWithImplementationWithout")]
			public void GenericInterfaceBaseWithImplementationWithout<T> () { }

			// === Properties ===
			[LogContains ("ImplementationClass.PropertyInterfaceBaseWithoutImplementationWith.get")]
			[LogContains ("ImplementationClass.PropertyInterfaceBaseWithoutImplementationWith.set")]
			[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			public Type PropertyInterfaceBaseWithoutImplementationWith { get; set; }


			// === RequiresUnreferencedCode ===
			[LogDoesNotContain ("ImplementationClass.RequiresUnreferencedCodeInterfaceBaseWithImplementationWith_")]
			[RequiresUnreferencedCode ("")]
			public void RequiresUnreferencedCodeInterfaceBaseWithImplementationWith_ () { }
			[ExpectedWarning ("IL2046", "ImplementationClass.RequiresUnreferencedCodeInterfaceBaseWithoutImplementationWith_")]
			[RequiresUnreferencedCode ("")]
			public void RequiresUnreferencedCodeInterfaceBaseWithoutImplementationWith_ () { }
		}


		interface IBaseImplementedInterface
		{
			Type ReturnValueBaseWithInterfaceWithout ();

			[RequiresUnreferencedCode ("")]
			void RequiresUnreferencedCodeBaseWithoutInterfaceWith ();
		}

		class BaseImplementsInterfaceViaDerived
		{
			[LogContains (
				"'DynamicallyAccessedMemberTypes' in 'DynamicallyAccessedMembersAttribute' on the return value of method 'Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.BaseImplementsInterfaceViaDerived.ReturnValueBaseWithInterfaceWithout()' " +
				"don't match overridden return value of method 'Mono.Linker.Tests.Cases.DataFlow.VirtualMethodHierarchyDataflowAnnotationValidation.IBaseImplementedInterface.ReturnValueBaseWithInterfaceWithout()'. " +
				"All overridden members must have the same 'DynamicallyAccessedMembersAttribute' usage.")]
			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
			public virtual Type ReturnValueBaseWithInterfaceWithout () => null;

			[ExpectedWarning ("IL2046", "BaseImplementsInterfaceViaDerived.RequiresUnreferencedCodeBaseWithoutInterfaceWith")]
			public virtual void RequiresUnreferencedCodeBaseWithoutInterfaceWith () { }
		}

		class DerivedWithInterfaceImplementedByBase : BaseImplementsInterfaceViaDerived, IBaseImplementedInterface
		{
		}


		interface ITwoInterfacesImplementedByOneMethod_One
		{
			Type ReturnValueInterfaceWithoutImplementationWith ();
		}

		interface ITwoInterfacesImplementedByOneMethod_Two
		{
			Type ReturnValueInterfaceWithoutImplementationWith ();
		}

		class ImplementationOfTwoInterfacesWithOneMethod : ITwoInterfacesImplementedByOneMethod_One, ITwoInterfacesImplementedByOneMethod_Two
		{
			[LogContains ("ITwoInterfacesImplementedByOneMethod_One.ReturnValueInterfaceWithoutImplementationWith")]
			[LogContains ("ITwoInterfacesImplementedByOneMethod_Two.ReturnValueInterfaceWithoutImplementationWith")]
			[return: DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicMethods)]
			public virtual Type ReturnValueInterfaceWithoutImplementationWith () => null;
		}
	}
}

namespace System
{
	// This verifies correct validation of the "this" parameter annotations
	class VirtualMethodHierarchyDataflowAnnotationValidationTypeTestBase : TestSystemTypeBase
	{
		[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors)]
		public virtual void ThisBaseWithDerivedWithout () { }
		[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors)]
		public virtual void ThisBaseWithDerivedWith_ () { }
		public virtual void ThisBaseWithoutDerivedWith () { }
	}

	class VirtualMethodHierarchyDataflowAnnotationValidationTypeTestDerived : VirtualMethodHierarchyDataflowAnnotationValidationTypeTestBase
	{
		[ExpectedWarning ("IL2094",
			"System.VirtualMethodHierarchyDataflowAnnotationValidationTypeTestDerived.ThisBaseWithDerivedWithout()",
			"System.VirtualMethodHierarchyDataflowAnnotationValidationTypeTestBase.ThisBaseWithDerivedWithout()")]
		public override void ThisBaseWithDerivedWithout () { }

		[LogDoesNotContain ("VirtualMethodHierarchyDataflowAnnotationValidationTypeTestDerived.ThisBaseWithDerivedWith_")]
		[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors)]
		public override void ThisBaseWithDerivedWith_ () { }

		[LogContains ("VirtualMethodHierarchyDataflowAnnotationValidationTypeTestDerived.ThisBaseWithoutDerivedWith")]
		[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors)]
		public override void ThisBaseWithoutDerivedWith () { }
	}
}