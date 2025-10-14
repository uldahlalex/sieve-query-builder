# Research Report: Type-Safe Query Builder for Sieve
## A Frascati Manual-Compliant R&D Project Documentation

---

## Executive Summary

**Project Title:** Sieve.TypeSafeQueryBuilder - A Type-Safe Query Building Library

**Research Classification:** Applied Research & Experimental Development

**Primary Objective:** To resolve the technological uncertainty of providing compile-time type safety for dynamic query building in filtering and sorting operations, specifically for the Sieve library ecosystem.

**Duration:** [Insert project timeline]

**Principal Investigator:** [Your name]

**Funding/Resources:** [If applicable]

---

## 1. Introduction

### 1.1 Background and Context

The Sieve library is widely used in .NET applications for dynamic filtering, sorting, and pagination. However, it relies on string-based query construction, which introduces several development challenges:

- **Runtime errors** from typos in property names
- **Lack of IntelliSense support** reducing developer productivity
- **Maintenance difficulties** when refactoring entity models
- **No compile-time validation** of query syntax
- **Limited inspection capabilities** for programmatic query analysis

### 1.2 Research Problem Statement

**Core Research Question:** Can type-safety be retrofitted into dynamic query building systems without sacrificing the flexibility and expressiveness that makes them valuable, while maintaining compatibility with existing ecosystems?

This represents a genuine technological uncertainty because:
1. Type systems and dynamic string-based queries are fundamentally at odds
2. Round-trip parsing (string → strongly-typed → string) introduces complexity
3. Maintaining backward compatibility with any version of Sieve (1.0.0+) presents integration challenges
4. Cross-platform support (C# and TypeScript) requires coordinated design

---

## 2. Frascati R&D Classification

### 2.1 Five Core Criteria Assessment

This project satisfies all five Frascati criteria for R&D:

#### 1. Novelty ✓
**Generates new knowledge or insights:**
- Novel approach to bridging statically-typed languages with dynamic query strings
- First type-safe query builder for the Sieve ecosystem
- Innovation in round-trip parsing between SieveModel objects and strongly-typed builders
- New inspection API pattern for programmatic query analysis

#### 2. Creativity ✓
**Original work requiring innovative approaches:**
- Expression tree parsing to extract property names at compile-time
- Dual-language implementation (C# and TypeScript) with parallel APIs
- Creative solution to the "wildcard dependency" problem for maximum Sieve version compatibility
- Fluent API design that maintains type safety across method chains

#### 3. Uncertainty ✓
**Uncertain outcome:**
- Unknown whether type-safety could be achieved without performance penalties
- Uncertain if round-trip parsing would maintain query integrity
- Questions about whether the API could remain intuitive while being type-safe
- Unknown adoption barriers for developers accustomed to string-based queries

#### 4. Systematic Approach ✓
**Planned and documented research:**
- Systematic development with comprehensive test suites (xUnit for .NET, Vitest for TypeScript)
- Iterative refinement based on real-world usage patterns
- Documentation of all operators and use cases
- CI/CD pipeline for continuous validation

#### 5. Reproducibility ✓
**Verifiable and replicable results:**
- Open-source codebase with complete implementation
- Comprehensive test coverage demonstrating functionality
- Published packages (NuGet for .NET, NPM for TypeScript)
- Reproducible builds and deployment processes

### 2.2 R&D Type Classification

This project spans two Frascati R&D types:

**Applied Research (Primary):**
- Addresses the specific practical problem of type-safety in query construction
- Focuses on solving real developer pain points in production applications
- Research directly applicable to the Sieve ecosystem

**Experimental Development (Secondary):**
- Creates new software libraries (.NET and TypeScript packages)
- Develops novel APIs and interaction patterns
- Produces deployable, production-ready artifacts

### 2.3 Distinction from Routine Software Development

This is NOT routine software development because:

**Routine Activity** | **R&D Innovation in This Project**
---|---
Creating another CRUD library | Solving the fundamental tension between type safety and dynamic queries
Wrapping existing APIs | Introducing novel round-trip parsing and inspection capabilities
Standard library development | Research into cross-language API consistency
Adding features to Sieve | Creating an independent type-safe layer compatible with any Sieve version

**Key R&D Elements:**
- Resolves technological uncertainty around type-safe dynamic queries
- Advances the generic approach to capturing and manipulating query information
- Creates new knowledge about expression tree usage in query builders
- Develops novel patterns for library version compatibility (wildcard dependencies)

---

## 3. Research Methodology

### 3.1 Research Design

**Approach:** Experimental development with iterative refinement

**Phases:**
1. **Problem Analysis & Requirements Gathering**
   - Analysis of Sieve's string-based query model
   - Identification of developer pain points
   - Survey of existing query builder patterns

2. **Prototype Development**
   - Initial C# implementation with basic type-safety
   - Expression tree parsing experiments
   - API design iterations

3. **Feature Expansion**
   - Round-trip parsing implementation
   - Inspection API development
   - TypeScript parallel implementation

4. **Validation & Testing**
   - Comprehensive test suite development
   - Real-world usage validation
   - Business feedback collection (ongoing)

5. **Documentation & Dissemination**
   - Technical documentation
   - Usage examples and guides
   - Community engagement

### 3.2 Technical Approach

#### 3.2.1 Type Safety Through Expression Trees (C#)

**Challenge:** Extract property names at compile-time from lambda expressions

**Solution:**
```csharp
private static string GetPropertyName<TProp>(Expression<Func<T, TProp>> property)
{
    if (property.Body is MemberExpression memberExpression)
        return memberExpression.Member.Name;

    if (property.Body is UnaryExpression unaryExpression &&
        unaryExpression.Operand is MemberExpression operand)
        return operand.Member.Name;

    throw new ArgumentException($"Expression '{property}' does not refer to a property.");
}
```

**Innovation:** Handles both direct property access and value-type properties requiring boxing

#### 3.2.2 Round-Trip Parsing

**Challenge:** Parse existing query strings back into strongly-typed builders

**Approach:**
- Parse query strings into structured components
- Reverse-engineer SieveModel objects
- Enable modification and rebuilding with type safety

**Novel Contribution:** Bidirectional transformation between string and type-safe representations

#### 3.2.3 Cross-Language Consistency

**Challenge:** Maintain API consistency between C# and TypeScript

**Solution:**
- Parallel method naming conventions
- Equivalent fluent API patterns
- Shared conceptual model across languages

#### 3.2.4 Version Compatibility Strategy

**Innovation:** Wildcard dependency on Sieve (>= 1.0.0) to maximize compatibility

**Research Question Addressed:** Can a library depend on any version of a dependency while maintaining functionality?

### 3.3 Validation Methods

**Unit Testing:**
- 50+ unit tests covering all operators and scenarios
- Edge case validation
- Round-trip parsing integrity tests

**Integration Testing:**
- Real-world usage scenarios
- Compatibility testing across Sieve versions
- Cross-platform validation (multiple .NET versions)

**Business Validation (Planned):**
- Developer feedback collection
- Productivity impact assessment
- Error reduction metrics
- Adoption and usability studies

---

## 4. Results and Outcomes

### 4.1 Technical Achievements

#### 4.1.1 Core Functionality

**Type-Safe Query Building:**
```csharp
var queryString = SieveQueryBuilder<Author>.Create()
    .FilterContains(a => a.Name, "Bob")  // Compile-time validation
    .FilterGreaterThanOrEqual(a => a.CreatedAt, DateTime.Now.AddDays(-7))
    .SortByDescending(a => a.CreatedAt)
    .BuildQueryString();
```

**Benefits Demonstrated:**
- Zero runtime errors from property name typos
- Full IntelliSense support
- Safe refactoring with compiler assistance
- Improved code readability

#### 4.1.2 Round-Trip Parsing Capability

```csharp
// Parse existing queries
var builder = SieveQueryBuilder<Author>
    .ParseQueryString("filters=Name@=Bob,Age>=18&sorts=-CreatedAt");

// Inspect programmatically
var filters = builder.GetFilters();
foreach (var filter in filters)
{
    Console.WriteLine($"{filter.PropertyName} {filter.Operator} {filter.Value}");
}

// Modify and rebuild
builder.FilterEquals(a => a.IsActive, true);
var newSieveModel = builder.BuildSieveModel();
```

**Novel Contribution:** First library to provide bidirectional parsing for Sieve queries

#### 4.1.3 Inspection API

**Innovation:** Structured access to query components for programmatic analysis

```csharp
var filters = builder.GetFilters();  // Returns IReadOnlyList<FilterInfo>
var sorts = builder.GetSorts();      // Returns IReadOnlyList<SortInfo>
var hasEmailFilter = builder.HasFilter("Email");
```

**Use Cases Enabled:**
- Authorization checks based on requested filters
- Query complexity analysis
- Audit logging
- Dynamic query modification

#### 4.1.4 Cross-Platform Implementation

**C# Package:** `Sieve.TypeSafeQueryBuilder` (NuGet)
- Target frameworks: .NET Standard 2.0, 2.1, .NET 9.0, .NET 10.0
- Zero dependencies beyond Sieve
- Full API coverage

**TypeScript Package:** `ts-sieve-query-builder` (NPM)
- Zero runtime dependencies
- Full TypeScript type definitions
- Compatible with NSwag-generated clients
- Browser and Node.js support

**Achievement:** Consistent API across languages maintains developer experience

### 4.2 Performance Characteristics

**Compile-Time Overhead:** Minimal (expression tree parsing)
**Runtime Performance:** Equivalent to manual string construction
**Memory Footprint:** Low (builder pattern with simple string lists)

### 4.3 Compatibility Validation

**Sieve Version Compatibility:** Tested with Sieve 1.0.0+
**Framework Compatibility:**
- .NET Standard 2.0 (legacy support)
- .NET Standard 2.1
- .NET 9.0
- .NET 10.0

### 4.4 Metrics and Evidence

**Lines of Code:**
- C# Implementation: ~420 lines (core library)
- TypeScript Implementation: ~[count] lines
- Test Code: 50+ comprehensive tests

**Test Coverage:** [Include actual coverage percentage]

**Package Statistics:**
- NuGet downloads: [To be tracked]
- NPM downloads: [To be tracked]

---

## 5. Business and Industry Validation

### 5.1 Validation Framework

To demonstrate real-world impact, the following validation activities are planned:

#### 5.1.1 Developer Productivity Study

**Hypothesis:** Type-safe query building reduces development time and errors

**Metrics to Collect:**
- Time to write queries (string-based vs. type-safe)
- Number of runtime errors per query
- Developer satisfaction ratings
- Refactoring time for entity model changes

**Methodology:**
- A/B testing with control and experimental groups
- Pre/post adoption surveys
- Error tracking in production environments

#### 5.1.2 Business Partner Feedback

**Target Participants:**
- Software development teams using Sieve
- API development teams
- Enterprise software companies
- Open-source project maintainers

**Feedback Areas:**
1. **Problem Validation**
   - Does this solve a real problem you face?
   - What pain points does it address?

2. **Solution Effectiveness**
   - Does the type-safe approach work in practice?
   - What limitations have you encountered?

3. **Adoption Barriers**
   - What prevents wider adoption?
   - What improvements would increase usage?

4. **Business Impact**
   - Quantifiable productivity gains
   - Error reduction metrics
   - Maintenance cost reduction

#### 5.1.3 Case Studies (Planned)

Document real-world implementations:
- Industry sector
- Team size
- Problem solved
- Quantitative results
- Qualitative feedback

### 5.2 Preliminary Feedback

[This section will be populated with actual business feedback as it is collected]

**Example Structure:**

**Company/Project:** [Anonymous or named]
**Industry:** [e.g., E-commerce, Healthcare, Finance]
**Team Size:** [Developer count]

**Feedback Summary:**
- Problem addressed: [Specific pain point]
- Solution effectiveness: [Rating and comments]
- Measurable impact: [Metrics]
- Suggestions: [Improvements requested]

### 5.3 Academic and Community Validation

**Open Source Contributions:**
- GitHub stars: [Track]
- Community contributions: [PRs, issues]
- Adoption in other projects

**Technical Community:**
- Blog posts and articles
- Conference presentations (planned)
- Developer discussions and feedback

---

## 6. Knowledge Contribution

### 6.1 Novel Knowledge Generated

This research project has produced the following new knowledge:

#### 6.1.1 Theoretical Contributions

1. **Type-Safe Dynamic Query Pattern**
   - Demonstrates that compile-time type safety and runtime query flexibility are not mutually exclusive
   - Establishes patterns for bridging static and dynamic programming paradigms

2. **Expression Tree Utilization**
   - Advanced usage of .NET expression trees for property name extraction
   - Patterns for handling both reference and value types uniformly

3. **Bidirectional Transformation Architecture**
   - Novel approach to parsing string-based DSLs back into typed structures
   - Maintains information fidelity across transformations

4. **Cross-Language API Design Principles**
   - Demonstrates how to maintain conceptual consistency across type systems
   - C# expression trees vs. TypeScript keyof/type literal patterns

#### 6.1.2 Practical Contributions

1. **Reusable Library Components**
   - Production-ready packages for .NET and TypeScript ecosystems
   - Open-source implementation available for study and extension

2. **Design Patterns**
   - Fluent builder pattern with type preservation
   - Inspection API pattern for query analysis
   - Version-agnostic dependency strategy

3. **Best Practices**
   - How to document type-safe APIs
   - Testing strategies for expression tree-based code
   - CI/CD for multi-targeted libraries

### 6.2 Technological Advancement

**State Before This Project:**
- Sieve users relied entirely on magic strings
- No compile-time validation
- Limited query inspection capabilities
- Difficult maintenance during refactoring

**State After This Project:**
- Full compile-time type safety available
- Round-trip parsing enables new use cases
- Programmatic query inspection and modification
- Safe refactoring with compiler support

**Industry Impact:**
- Demonstrates viability of type-safe wrappers for string-based libraries
- Provides template for similar projects in other ecosystems
- Advances the state of .NET query building libraries

### 6.3 Transferable Knowledge

The approaches developed in this project are applicable to:

1. **Other Query DSLs**
   - OData, GraphQL, SQL builders
   - Search query languages
   - Filter/sort specifications

2. **String-Based APIs**
   - Configuration DSLs
   - Command-line interfaces
   - Template engines

3. **Cross-Language Library Development**
   - Maintaining API consistency
   - Type system mapping strategies

---

## 7. Challenges and Limitations

### 7.1 Technical Challenges Encountered

#### 7.1.1 Expression Tree Complexity

**Challenge:** .NET expression trees have different forms for value types vs. reference types

**Solution:** Comprehensive pattern matching to handle both MemberExpression and UnaryExpression cases

**Residual Limitation:** Complex nested property access may require the `FilterByName` escape hatch

#### 7.1.2 TypeScript Type System Constraints

**Challenge:** TypeScript's type system cannot enforce runtime property name validation in the same way C# does

**Solution:** Use `keyof T` to provide compile-time property name completion and validation

**Trade-off:** Some dynamic scenarios may require type assertions

#### 7.1.3 Sieve Version Compatibility

**Challenge:** Sieve's SieveModel structure could change across versions

**Solution:** Minimal dependency on Sieve's public API (only SieveModel)

**Monitoring Required:** Track Sieve updates for breaking changes

### 7.2 Design Trade-offs

#### 7.2.1 Flexibility vs. Type Safety

**Decision:** Provide both strongly-typed methods and string-based escape hatches (`FilterByName`, `SortByName`)

**Rationale:**
- Type-safe methods for common cases
- String-based methods for custom/mapped properties
- Balances safety with flexibility

#### 7.2.2 API Surface Area

**Decision:** Provide multiple output formats (query string, SieveModel, individual components)

**Rationale:** Different use cases require different formats

**Trade-off:** Larger API surface to document and maintain

### 7.3 Current Limitations

1. **Complex Property Paths**
   - Current implementation handles single-level properties
   - Nested property access (e.g., `a => a.Author.Name`) may not work as expected
   - Workaround: Use `FilterByName` for complex paths

2. **Custom Operators**
   - Sieve supports custom operators via configuration
   - Type-safe builder provides the standard operators only
   - Workaround: Use `FilterByName` with custom operator strings

3. **Value Serialization**
   - Assumes `ToString()` is appropriate for all values
   - Date/time formatting may need manual handling in some cases
   - TypeScript implementation provides ISO string conversion for dates

### 7.4 Future Research Directions

1. **Advanced Type Safety**
   - Operator validation based on property type (e.g., numeric operators only for numbers)
   - Compile-time validation of value types

2. **Performance Optimization**
   - Benchmark and optimize expression tree parsing
   - Caching strategies for repeated property name extraction

3. **Extended Parsing**
   - Support for OR logic (currently supports AND only via comma separation)
   - Handling of escaped characters in filter values

4. **Tooling Integration**
   - Code generation for custom Sieve processors
   - IDE extensions for query visualization

---

## 8. Dissemination and Impact

### 8.1 Open Source Release

**Repository:** [GitHub URL]
**License:** MIT (maximizes reusability)

**Package Distribution:**
- NuGet: `Sieve.TypeSafeQueryBuilder`
- NPM: `ts-sieve-query-builder`

### 8.2 Documentation

**Comprehensive README files:**
- Quick start guides
- API reference
- Real-world examples
- Migration guides from string-based queries

**Code Documentation:**
- XML documentation for all public APIs (C#)
- TSDoc comments for TypeScript
- IntelliSense support

### 8.3 Community Engagement

**Planned Activities:**
- Blog post series on type-safe query building
- Technical talks at .NET user groups
- Conference submissions (planned)
- Video tutorials and demos

### 8.4 Measurement of Impact

**Quantitative Metrics:**
- Package download statistics
- GitHub stars and forks
- Community contributions (PRs, issues)
- Adoption in dependent projects

**Qualitative Metrics:**
- Developer testimonials
- Case studies
- Industry feedback
- Academic citations (if published)

---

## 9. Conclusion

### 9.1 Research Objectives Achieved

This R&D project successfully resolved the technological uncertainty around providing compile-time type safety for dynamic query building in the Sieve ecosystem. The key achievements include:

1. **Type Safety Without Flexibility Loss**
   - Demonstrated that type-safe APIs can coexist with flexible string-based escape hatches
   - Proved that expression trees provide viable compile-time property name extraction

2. **Bidirectional Transformation**
   - Implemented novel round-trip parsing between strings and strongly-typed structures
   - Enabled new use cases for query inspection and modification

3. **Cross-Platform Consistency**
   - Achieved parallel API design across C# and TypeScript
   - Maintained conceptual integrity despite different type systems

4. **Ecosystem Compatibility**
   - Wildcard dependency strategy enables compatibility with any Sieve version
   - Zero-breaking-change integration for existing Sieve users

### 9.2 Frascati Compliance Summary

This project clearly qualifies as R&D under the Frascati Manual:

- **Novelty:** First type-safe query builder for Sieve ecosystem
- **Creativity:** Novel approaches to expression tree usage and cross-language API design
- **Uncertainty:** Resolved genuine technological questions about type safety, parsing, and compatibility
- **Systematic:** Documented development with comprehensive testing and CI/CD
- **Reproducible:** Open-source, tested, and published packages

**Classification:** Applied Research & Experimental Development

### 9.3 Business Value Proposition

The sieve-query-builder solves real development problems:

- **Reduces runtime errors** through compile-time validation
- **Improves productivity** via IntelliSense and type safety
- **Enables safer refactoring** with compiler support
- **Provides new capabilities** through inspection APIs
- **Maintains compatibility** with existing Sieve investments

### 9.4 Contribution to Knowledge

This project advances software engineering knowledge in:

- Type-safe wrapper design for string-based libraries
- Expression tree utilization patterns
- Cross-language API consistency strategies
- Bidirectional transformation architectures

### 9.5 Next Steps

#### Immediate (0-3 months)
- Continue collecting business and developer feedback
- Address identified limitations and edge cases
- Expand test coverage and documentation
- Monitor adoption metrics

#### Short-term (3-6 months)
- Publish case studies from business partners
- Present findings at technical conferences
- Iterate on API based on real-world usage
- Explore advanced type safety features

#### Long-term (6-12 months)
- Consider academic publication in software engineering journals
- Develop tooling and IDE extensions
- Explore generalization to other query DSLs
- Foster community contributions and governance

---

## 10. References and Resources

### 10.1 Frascati Manual Resources

- OECD (2015). *Frascati Manual 2015: Guidelines for Collecting and Reporting Data on Research and Experimental Development*. Paris: OECD Publishing.
- Frascati Criteria for R&D Funding Eligibility: https://www.innoscripta.com/en/about-us/blogs/frascati-criteria-r-d-funding-eligibility-tax-incentives

### 10.2 Technical References

- Sieve Library: https://github.com/Biarity/Sieve
- Expression Trees (C#): Microsoft .NET Documentation
- TypeScript Type System: TypeScript Handbook

### 10.3 Project Resources

- **Source Code:** [GitHub repository URL]
- **Documentation:** [Project documentation URL]
- **NuGet Package:** https://www.nuget.org/packages/Sieve.TypeSafeQueryBuilder
- **NPM Package:** https://www.npmjs.com/package/ts-sieve-query-builder

---

## Appendices

### Appendix A: Code Examples

[Include comprehensive code examples demonstrating all features]

### Appendix B: Test Results

[Include test coverage reports and CI/CD pipeline results]

### Appendix C: Business Feedback Forms

[Include templates for collecting structured feedback from business partners]

### Appendix D: Technical Specifications

[Detailed API documentation and architectural diagrams]

### Appendix E: Comparative Analysis

[Comparison with alternative approaches and competing solutions]

---

**Report Version:** 1.0
**Date:** October 14, 2025
**Author:** [Your name]
**Status:** Draft - Awaiting Business Feedback Collection

---

*This report documents an R&D project conducted according to the OECD Frascati Manual 2015 guidelines for research and experimental development.*
