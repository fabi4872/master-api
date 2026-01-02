using MasterApi.Domain.Primitives;

namespace MasterApi.Domain.Errors;

public static class DomainErrors
{
    public static class Request
    {
        public static readonly Error Unspecified = new("request.unspecified", "An unspecified error has occurred.");
        public static readonly Error NotFound = new("request.notfound", "The requested resource was not found.");
        public static readonly Error ValidationError = new("request.validation", "One or more validation errors occurred.");
    }
    
    public static class User
    {
        public static readonly Error NotFound = new("user.notfound", "The user was not found.");
        public static readonly Error InvalidCredentials = new("user.invalidcredentials", "Invalid credentials.");
        public static readonly Error NameRequired = new("user.namerequired", "The name is required.");
        public static readonly Error EmailRequired = new("user.emailrequired", "The email is required.");
        public static readonly Error EmailInvalid = new("user.emailinvalid", "The email format is invalid.");
        public static readonly Error EmailAlreadyExists = new("user.emailalreadyexists", "The email is already in use.");
    }
}
