# Proposal: Improve TestSuites Domain

## Background
The `TestSuites` domain currently provides basic functionality for managing test suites and cases. However, it lacks some flexibility in state management (e.g., reverting to Draft), comprehensive execution tracking (start/end times), and robust bulk operation feedback (import results). Additionally, some code elements lack sufficient documentation.

## Goals
1.  **Enhance State Management:** Allow `Ready` suites to be reset to `Draft`. Allow `Draft` suites to be `Archived`.
2.  **Improve Execution Tracking:** Explicitly track `ExecutionStartTime` and `ExecutionEndTime` for `TestSuite`.
3.  **Robust Import:** Update `ImportTestCasesAsync` to return a detailed result object indicating success/failure for each imported case.
4.  **Documentation:** Add and improve XML comments for all public members in the `TestSuites` domain.
5.  **Validation:** Ensure `UpdateTestCase` prevents duplicate titles within the suite.

## Scope
-   `src/VoloAbp.OTel.Domain/TestSuites/Aggregates/TestSuite.cs`
-   `src/VoloAbp.OTel.Domain/TestSuites/Aggregates/TestCase.cs`
-   `src/VoloAbp.OTel.Domain/TestSuites/TestSuiteManager.cs`
-   `src/VoloAbp.OTel.Domain/TestSuites/ITestSuiteManager.cs`
-   `src/VoloAbp.OTel.Domain/TestSuites/Datas/TestCaseImportResult.cs` (New)

## Design
-   **TestSuite State Machine:**
    -   Add `ResetToDraft()`: Ready -> Draft.
    -   Update `Archive()`: Allow Draft -> Archived.
-   **Execution Tracking:**
    -   Add `DateTime? ExecutionStartTime` and `DateTime? ExecutionEndTime` to `TestSuite`.
    -   Update `Execute()` to set `ExecutionStartTime`.
    -   Update `CompleteExecution()` and `FailExecution()` to set `ExecutionEndTime`.
-   **Import Result:**
    -   Introduce `TestCaseImportResult` class to hold `TotalCount`, `SuccessCount`, `FailedCount`, and a list of `FailedImport` (Row/Index, Error).
    -   Change `ImportTestCasesAsync` signature to return `TestCaseImportResult`.
