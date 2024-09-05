using Dapper;
using System.Data;
using Website.Models;
using Website.Persistence;

namespace Website.Repositories;

/// <summary>
/// Repository for handling attendances (weak entities) from a database.
/// </summary>
public class AttendancesRepository : RepositoryBase
{
    /// <summary>
    /// Initialises a new instance of the <see cref="AttendancesRepository"/> class.
    /// </summary>
    /// <param name="connectionProvider">The connection provider to use.</param>
    public AttendancesRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
    {
    }

    /// <summary>
    /// Creates the attendances table, if it doesn't already exist.
    /// </summary>
    /// <param name="cancellationToken">A token which can be used to cancel asynchronous operations.</param>
    /// <returns>An awaitable task</returns>
    public async Task CreateTableIfNotExistsAsync(CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(
            @"
                CREATE TABLE IF NOT EXISTS [Attendances] (
                    [Id] integer primary key,
                    [EmployeeID] int NOT NULL,
                    [EventID] int NOT NULL,
                    FOREIGN KEY ([EmployeeID]) REFERENCES [Employee]([Id]),
                    FOREIGN KEY ([EventID]) REFERENCES [Event]([Id])
                );
            ",
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        await Connection.ExecuteAsync(command);
    }

    /// <summary>
    /// Gets all attendances.
    /// </summary>
    /// <param name="cancellationToken">A token which can be used to cancel asynchronous operations.</param>
    /// <returns>An awaitable task whose result is the attendances found.</returns>
    public async Task<IReadOnlyCollection<Attendance>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var command = new CommandDefinition(
            @"
                SELECT  [A].[Id],
                        [A].[EmployeeID],
                        [A].[EventID]
                FROM    [Attendances] AS [A]
                WHERE   (
                            @IncludeHistoricEvents = 1
                            OR  [E].[EndDateTime] > DATE('now')
                        );
            ",
            parameters: new
            {
                
            },
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        var attendances = await Connection.QueryAsync<Attendance>(command);

        return attendances
            .ToArray();
    }

    /// <summary>
    /// Gets an attendance by ID.
    /// </summary>
    /// <param name="id">The ID of the attendance to get.</param>
    /// <param name="cancellationToken">A token which can be used to cancel asynchronous operations.</param>
    /// <returns>An awaitable task whose result is the attendance if found, otherwise <see langword="null"/>.</returns>
    public async ValueTask<Event?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var command = new CommandDefinition(
            @"
                SELECT  [A].[Id],
                        [A].[EmployeeID],
                        [A].[EventID],
                FROM    [Attendance] AS [A]
                WHERE   [A].[Id] = @Id;
            ",
            parameters: new
            {
                Id = id
            },
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        return await Connection.QuerySingleOrDefaultAsync<Event>(command);
    }

    /// <summary>
    /// Determines whether an attendance with the given ID exists.
    /// </summary>
    /// <param name="id">The ID of the attendance to check.</param>
    /// <param name="cancellationToken">A token which can be used to cancel asynchronous operations.</param>
    /// <returns>An awaitable task whose result indicates whether the attendance exists.</returns>
    public async ValueTask<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        var command = new CommandDefinition(
            @"SELECT  CAST(CASE
                     WHEN EXISTS (
                                     SELECT 1
                                     FROM   [Attendance] AS [A]
                                     WHERE  [A].Id = @Id
                                 ) THEN 1
                     ELSE 0
                 END AS bit);
            ",
            parameters: new
            {
                Id = id
            },
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        return await Connection.ExecuteScalarAsync<bool>(command);
    }

    /// <summary>
    /// Creates a new attendance.
    /// </summary>
    /// <param name="attendance">The attendance to create. The <see cref="Attendance.Id"/> is ignored.</param>
    /// <returns>An awaitable task that results in the created attendance, with its Id.</returns>
    public async Task<Attendance> CreateAsync(Attendance @attendance, CancellationToken cancellationToken = default)
    {
        var command = new CommandDefinition(
            @"
                INSERT INTO [Attendance]
                (
                    [Id],
                    [EmployeeID],
                    [EventID]
                )
                VALUES
                (
                    @EmployeeID,
                    @EventID
                );

                SELECT  [A].[Id],
                        [A].[EmployeeID],
                        [A].[EventID]
                FROM    [Attendance] AS [A]
                WHERE   [A].[Id] = last_insert_rowid();
            ",
            parameters: new
            {
                @attendance.EmployeeID,
                @attendance.EventID
            },
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        var createdAttendance = await Connection.QuerySingleAsync<Attendance>(command);

        return createdAttendance;
    }

    /// <summary>
    /// Updates an existing attendance.
    /// NOTE: This shall not update in the same manner as Employees or Events, but is including to be refactored later
    /// </summary>
    /// <param name="attendance">The updated attendance details. The <see cref="Attendance.Id"/> should be the Id of the attendance to update.</param>
    /// <returns>An awaitable task that results in the updated attendance.</returns>
    public async Task<Attendance> UpdateAsync(Attendance @attendance, CancellationToken cancellationToken = default)
    {
        var command = new CommandDefinition(
            @"
                UPDATE  [Attendance]
                SET     [EmployeeID] = @EmployeeID,
                        [EventID] = @EventID
                WHERE   [Id] = @Id;

                SELECT  [A].[Id],
                        [A].[EmployeeID],
                        [A].[EventID]
                FROM    [Attendance] AS [A]
                WHERE   [A].[Id] = @Id;
            ",
            parameters: new
            {
                @attendance.Id,
                @attendance.EmployeeID,
                @attendance.EventID
            },
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);


        var updatedAttendance = await Connection.QuerySingleAsync<Attendance>(command);

        return updatedAttendance;
    }

    /// <summary>
    /// Deletes an existing attendance.
    /// </summary>
    /// <param name="attendanceId">The ID of the attendance to delete.</param>
    /// <returns>An awaitable task.</returns>
    public async Task DeleteAsync(int attendanceId, CancellationToken cancellationToken = default)
    {
        var command = new CommandDefinition(
            @"
                DELETE FROM [Attendance]
                WHERE   [Id] = @Id;
            ",
            parameters: new
            {
                Id = attendanceId
            },
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        await Connection.ExecuteAsync(command);
    }
}