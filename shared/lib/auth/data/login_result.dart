sealed class LoginResult {
  const LoginResult();
}

class LoginAuthenticated extends LoginResult {
  final String accessToken;
  const LoginAuthenticated(this.accessToken);
}

class LoginTwoFactorRequired extends LoginResult {
  final String challengeId;
  const LoginTwoFactorRequired(this.challengeId);
}
