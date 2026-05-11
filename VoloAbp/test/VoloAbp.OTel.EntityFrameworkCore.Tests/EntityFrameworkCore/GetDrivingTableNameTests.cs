using Shouldly;
using VoloAbp.OTel.EntityFrameworkCore;
using Xunit;

namespace VoloAbp.OTel.EntityFrameworkCore.Tests;

public class GetDrivingTableNameTests : OTelEntityFrameworkCoreTestBase
{
    private readonly TaggedTraceidCommandInterceptor _interceptor;

    public GetDrivingTableNameTests()
    {
        _interceptor = GetRequiredService<TaggedTraceidCommandInterceptor>();
    }

    [Fact]
    public void SELECT_Simple_Backtick()
    {
        var sql = "SELECT `a`.`Id`, `a`.`Name` FROM `AppUsers` AS `a` WHERE `a`.`Id` = 1";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("SELECT");
        table.ShouldBe("AppUsers");
    }

    [Fact]
    public void SELECT_Simple_Bracket()
    {
        var sql = "SELECT [a].[Id] FROM [AppUsers] AS [a]";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("SELECT");
        table.ShouldBe("AppUsers");
    }

    [Fact]
    public void SELECT_Simple_NoQuote()
    {
        var sql = "SELECT Id, Name FROM AppUsers WHERE Id = 1";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("SELECT");
        table.ShouldBe("AppUsers");
    }

    [Fact]
    public void SELECT_WithJoin_OnlyDrivingTable()
    {
        var sql = "SELECT `a`.`Id`, `b`.`Title` FROM `AppUsers` AS `a` INNER JOIN `AppBooks` AS `b` ON `a`.`Id` = `b`.`AuthorId`";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("SELECT");
        table.ShouldBe("AppUsers");
    }

    [Fact]
    public void INSERT_Into_Backtick()
    {
        var sql = "INSERT INTO `AppUsers` (`Name`, `Email`) VALUES (@p0, @p1)";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("INSERT");
        table.ShouldBe("AppUsers");
    }

    [Fact]
    public void INSERT_Into_Bracket()
    {
        var sql = "INSERT INTO [AppUsers] ([Name]) VALUES (@p0)";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("INSERT");
        table.ShouldBe("AppUsers");
    }

    [Fact]
    public void UPDATE_Backtick()
    {
        var sql = "UPDATE `AppUsers` SET `Name` = @p0 WHERE `Id` = @p1";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("UPDATE");
        table.ShouldBe("AppUsers");
    }

    [Fact]
    public void UPDATE_Bracket()
    {
        var sql = "UPDATE [AppUsers] SET [Name] = @p0 WHERE [Id] = @p1";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("UPDATE");
        table.ShouldBe("AppUsers");
    }

    [Fact]
    public void DELETE_From_Backtick()
    {
        var sql = "DELETE FROM `AppUsers` WHERE `Id` = @p0";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("DELETE");
        table.ShouldBe("AppUsers");
    }

    [Fact]
    public void DELETE_From_Bracket()
    {
        var sql = "DELETE FROM [AppUsers] WHERE [Id] = @p0";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("DELETE");
        table.ShouldBe("AppUsers");
    }

    [Fact]
    public void INSERT_MultiStatement_OnlyFirst()
    {
        var sql = "INSERT INTO `AppUsers` (`Name`) VALUES (@p0);\nSELECT `Id` FROM `AppUsers` WHERE changes() = 1 AND `rowid` = last_insert_rowid()";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("INSERT");
        table.ShouldBe("AppUsers");
    }

    [Fact]
    public void NullInput_ReturnsNulls()
    {
        var (action, table) = _interceptor.GetDrivingTableName(null!);
        action.ShouldBeNull();
        table.ShouldBeNull();
    }

    [Fact]
    public void EmptyInput_ReturnsNulls()
    {
        var (action, table) = _interceptor.GetDrivingTableName("");
        action.ShouldBeNull();
        table.ShouldBeNull();
    }

    [Fact]
    public void WhitespaceInput_ReturnsNulls()
    {
        var (action, table) = _interceptor.GetDrivingTableName("   ");
        action.ShouldBeNull();
        table.ShouldBeNull();
    }

    [Fact]
    public void UnrecognizedSql_ReturnsNulls()
    {
        var (action, table) = _interceptor.GetDrivingTableName("MERGE INTO AppUsers ...");
        action.ShouldBeNull();
        table.ShouldBeNull();
    }

    [Fact]
    public void MSSql_Select_Simple()
    {
        var sql = "SELECT  [GroupBy1].[A1] AS [C1] FROM ( SELECT      COUNT(1) AS [A1]     FROM [dbo].[Test] AS [Extent1] )  AS [GroupBy1]";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("SELECT");
        table.ShouldBe("Test");
    }

    [Fact]
    public void MSSql_INSERT_Simple()
    {
        var sql = "INSERT [dbo].[InfAuditLogs]([TenantId], [UserId], [ServiceName], [MethodName], [Parameters], [ReturnValue], [ExecutionTime], [ExecutionDuration], [ClientIpAddress], [ClientName], [BrowserInfo], [Exception], [ImpersonatorUserId], [ImpersonatorTenantId], [CustomData]) VALUES (@0, @1, @2, @3, @4, NULL, @5, @6, @7, @8, @9, NULL, NULL, @10, NULL) SELECT [Id] FROM [dbo].[InfAuditLogs] WHERE @@ROWCOUNT > 0 AND [Id] = scope_identity()";
        var (action, table) = _interceptor.GetDrivingTableName(sql);
        action.ShouldBe("INSERT");
        table.ShouldBe("InfAuditLogs");
    }
}
