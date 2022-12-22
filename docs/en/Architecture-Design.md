The purpose of this document is to outline the architecture design decisions made for this project, including the use of
Domain-Driven Design, Test-Driven Development, SOLID principles, and the ABP framework.

# Domain Driven Design

Domain-Driven Design (DDD) is an approach to software development that focuses on creating a domain model that
accurately reflects the problem domain of the software being developed. The idea behind DDD is that by building a model
of the domain and using it as the basis for designing the software, we can create software that is more closely aligned
with the needs and requirements of the domain experts, and therefore more likely to be successful in meeting the needs
of the business.

# Test Driven Development

Test-Driven Development (TDD) is a software development process in which tests are written for a piece of code before
the code is written. The idea behind TDD is that by writing tests first, we can ensure that the code we write is of high
quality and meets the requirements of the business.

This project uses a combination of tools and best practices for unit testing, including xUnit, Shouldly, FakeItEasy,
and the AAA (Arrange-Act-Assert) convention.

## xUnit

xUnit is a popular unit testing framework for .NET applications that provides a set of tools and libraries for
writing and running tests. You can learn more about it and download it from the official website: https://xunit.net/

## Shouldly

Shouldly is an assertion library that provides a more human-readable syntax for writing test assertions. It makes it
easier to write clear and concise test cases that are easy to read and understand. You can learn more about Shouldly and
download it from the official website: https://shouldly.readthedocs.io/

## FakeItEasy

FakeItEasy is a library for creating fake objects (also known as mocks or stubs) in .NET applications. It can be
used to isolate units of code for testing and to simulate the behavior of dependencies. You can learn more about
FakeItEasy and download it from the official website: https://fakeiteasy.github.io/

## Arrange-Act-Assert

The AAA (Arrange-Act-Assert) convention is a common pattern for organizing unit tests. It involves setting up the test
context (arrange), executing the test (act), and verifying the results (assert). Using the AAA convention helps to
ensure that test cases are well-structured and easy to understand. You can learn more about the AAA convention and how
it is used in unit testing from the following resources:

- https://xunitpatterns.com/Arrange-Act-Assert.html
- https://blog.testproject.io/2019/01/21/the-aaa-pattern-arrange-act-assert/

# SOLID

SOLID is a set of five design principles for object-oriented programming that were introduced by Robert C. Martin. These
principles are:

- Single Responsibility Principle (SRP): A class should have only one reason to change.
- Open-Closed Principle (OCP): Software entities (classes, modules, functions, etc.) should be open for extension but
  closed for modification.
- Liskov Substitution Principle (LSP): Subtypes must be substitutable for their base types.
- Interface Segregation Principle (ISP): Clients should not be forced to depend on interfaces they do not use.
- Dependency Inversion Principle (DIP): High-level modules should not depend on low-level modules. Both should depend on
  abstractions.

# ABP Framework

The Quick Setup project adopted and utilise the ABP framework (short for "ASP.NET Boilerplate") as it provides a
modular, multi-layer application framework based on the latest dotnet core. It is designed to help developers create
scalable, high-performance, and maintainable applications by providing a set of pre-built, tested, and well-documented
components and tools.

## Application vs Domain Service Models

The following naming convention will be adopted to provide clarity and avoid conflicting model and namespaces names
between Application and Domain services:

### Folder Structure

The following naming convention and rules are being used to improve the consistency and maintainability of the project
over time.

- The domain and application service are divided into a dedicated folder and namespace.
- The folder shall be the plural of the domain entity e.g `Products`
- All input and output data transfer objects are placed into a subfolder called `Dto`
- AutoMapper is used to maintain mapping information between domain and application models
- In instances where custom mapping is required extension methods shall be used.
- All extension classes shall be placed into a subfolder called `Extensions`
- All application service operations should end with `Async`

### Naming Convention

The table below outlines the adopted suffix naming convention.

| Layer | Parameters | Return |
|--|--|--|
| Application Service | RequestDto | Dto |
| Domain Service | Input | Output |

### Example

The following is an example of the naming convention used to identify models naming convention for an operation that
returns a list of `Cards`

#### Domain Service

```csharp
public class CardListInput
{
    public string Quert { get; set; }
    public string DeptAndCC { get; set; }
    ...
}

public class CardListOutput
{
    public IReadOnlyList<CardOutput> Cards { get; set; }
    ...
}

public interface ICardDomainService : IDomainService
{
    Task<CardListOutput> GetListAsync(CardListInput input)
}
```

#### Application Service

```csharp
public class CardListRequestDto
{
    public string Filter { get; set; }
    public string CostCentre { get; set; }
    ...
}

public class CardListDto
{
    public IReadOnlyList<CardDto> Cards { get; set; }
    ...
}

public interface ICardAppService : IApplicationService
{
    Task<CardListDto> GetListAsync(CardListRequestDto request)
}
```

# CommandLineParser Library

The CommandLineParser library is a popular open-source library that provides a simple and flexible way to parse
command-line arguments in .NET applications. It allows developers to define a set of valid command-line options and
arguments, and then parse and validate user input against these definitions.

There are several benefits to using the CommandLineParser library in this project:

- Ease of use: The CommandLineParser library is easy to use and requires minimal setup. It provides a simple and
  intuitive API for defining command-line options and arguments, and for parsing and validating user input.
- Customization: The CommandLineParser library allows developers to customize the behavior of the parser to suit the
  needs of their specific project. For example, developers can specify whether options are required or optional, whether
  they accept values, and how those values should be parsed.
- Flexibility: The CommandLineParser library is flexible and can handle a wide range of command-line input scenarios. It
  supports both single-letter options (e.g. "-v") and long options (e.g. "--verbose"), as well as positional arguments
  and mixed options and arguments.
- Internationalization: The CommandLineParser library supports localization and internationalization, allowing
  developers to provide user-facing messages in different languages.

Overall, the CommandLineParser library is a valuable tool for any .NET project that needs to parse and validate
command-line input from users.

# Modularity

# Repository

There are a few reasons why it might make sense to save data repositories for domain aggregates and entities in a YAML
file, rather than using a binary file format like SQLite:

- Git is optimized for text files: Git is a version control system that is designed to manage and track changes to text
  files. When you commit and push changes to a Git repository, Git stores the changes in a compact and efficient way
  that minimizes the amount of data that needs to be transferred. This can be more efficient than transferring large
  binary files, which may require more data to be transferred with each update.
- YAML is a widely-supported format: YAML (YAML Ain't Markup Language) is a data serialization format that is used for
  storing and exchanging data. It is supported by many programming languages and tools, so it is easy to work with and
  integrate into different systems.
- YAML files are human-readable: YAML files are stored in a plain text format that is easy for humans to read and
  understand. This can be helpful when reviewing changes to the data repository, or when debugging issues with the data.
- YAML files may be smaller and faster to load: Depending on the size and complexity of the data in the repository,
  storing it in a YAML file may result in a smaller file size and faster loading times compared to a binary file format
  like SQLite.

Overall, using a YAML file to store data repositories for domain aggregates and entities may be a good choice when
working with Git, as it can take advantage of Git's optimizations for text files and minimize repository bloat over
time. It may also be a convenient and widely-supported format that is easy to work with and debug.
