namespace Peritos.Common.Abstractions
{
    /// <summary>
    /// Interface for the request context. This is used to pass information about the current request.
    /// </summary>
    public interface IRequestContext
    {
        /// <summary>
        /// Gets a value indicating whether the current request is authenticated.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets the ID of the authenticated user, if available.
        /// </summary>
        int? UserId { get; }

        /// <summary>
        /// Gets the role ID of the authenticated user, if available.
        /// </summary>
        int? RoleId { get; }

        /// <summary>
        /// Gets or sets the email address of the authenticated user.
        /// </summary>
        string UserEmail { get; set; }

        /// <summary>
        /// Gets the security token associated with the current request, if any.
        /// </summary>
        string? Token { get; }
    }
}
