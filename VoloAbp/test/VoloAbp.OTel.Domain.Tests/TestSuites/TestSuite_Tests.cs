using System;
using Shouldly;
using VoloAbp.OTel.TestSuites.Aggregates;
using VoloAbp.OTel.TestSuites.Enums;
using Xunit;

namespace VoloAbp.OTel.Domain.Tests.TestSuites;

public class TestSuite_Tests
{
    public TestSuite_Tests()
    {
    }

    [Fact]
    public void Should_Set_ExecutionStartTime_When_Executed()
    {
        // Arrange
        var testSuite = new TestSuite(
            Guid.NewGuid(),
            "Test Suite 1",
            "Description",
            "PROJ",
            "1.0.0",
            new TestConfiguration(30, 3)
        );

        testSuite.AddTestCase("Case 1", "Desc", "Step", "Result");
        testSuite.MarkAsReady();

        // Act
        testSuite.Execute();

        // Assert
        testSuite.Status.ShouldBe(TestSuiteStatus.Running);
        testSuite.ExecutionStartTime.ShouldNotBeNull();
        testSuite.ExecutionEndTime.ShouldBeNull();
    }

    [Fact]
    public void Should_Set_ExecutionEndTime_When_Completed()
    {
        // Arrange
        var testSuite = new TestSuite(
            Guid.NewGuid(),
            "Test Suite 1",
            "Description",
            "PROJ",
            "1.0.0",
            new TestConfiguration(30, 3)
        );

        testSuite.AddTestCase("Case 1", "Desc", "Step", "Result");
        testSuite.MarkAsReady();
        testSuite.Execute();

        // Act
        testSuite.CompleteExecution();

        // Assert
        testSuite.Status.ShouldBe(TestSuiteStatus.Completed);
        testSuite.ExecutionEndTime.ShouldNotBeNull();
        testSuite.ExecutionEndTime.Value.ShouldBeGreaterThanOrEqualTo(testSuite.ExecutionStartTime.Value);
    }

    [Fact]
    public void Should_Reset_To_Draft_From_Ready()
    {
        // Arrange
        var testSuite = new TestSuite(
            Guid.NewGuid(),
            "Test Suite 1",
            "Description",
            "PROJ",
            "1.0.0",
            new TestConfiguration(30, 3)
        );

        testSuite.AddTestCase("Case 1", "Desc", "Step", "Result");
        testSuite.MarkAsReady();
        testSuite.Status.ShouldBe(TestSuiteStatus.Ready);

        // Act
        testSuite.ResetToDraft();

        // Assert
        testSuite.Status.ShouldBe(TestSuiteStatus.Draft);
    }

    [Fact]
    public void Should_Archive_From_Draft()
    {
        // Arrange
        var testSuite = new TestSuite(
            Guid.NewGuid(),
            "Test Suite 1",
            "Description",
            "PROJ",
            "1.0.0",
            new TestConfiguration(30, 3)
        );

        testSuite.Status.ShouldBe(TestSuiteStatus.Draft);

        // Act
        testSuite.Archive();

        // Assert
        testSuite.Status.ShouldBe(TestSuiteStatus.Archived);
    }

    [Fact]
    public void Should_Not_Allow_Duplicate_Title_In_Update()
    {
        // Arrange
        var testSuite = new TestSuite(
            Guid.NewGuid(),
            "Test Suite 1",
            "Description",
            "PROJ",
            "1.0.0",
            new TestConfiguration(30, 3)
        );

        testSuite.AddTestCase("Case 1", "Desc", "Step", "Result");
        testSuite.AddTestCase("Case 2", "Desc", "Step", "Result");
        
        // Find Case 1
        var case1Id = Guid.Empty;
        foreach(var tc in testSuite.TestCases)
        {
            if (tc.Title == "Case 1")
            {
                case1Id = tc.Id;
                break;
            }
        }

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            testSuite.UpdateTestCase(case1Id, title: "Case 2");
        });
    }
}