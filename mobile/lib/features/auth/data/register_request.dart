class RegisterRequest {
  String? firstName;
  String? lastName;
  String? username;
  String? email;
  String? password;
  String? confirmPassword;
  String? countryIso2;
  String? timeZoneId;

  RegisterRequest({
    this.firstName,
    this.lastName,
    this.username,
    this.email,
    this.password,
    this.confirmPassword,
    this.countryIso2,
    this.timeZoneId,
  });

  Map<String, dynamic> toJson() {
    return {
      'firstName': firstName,
      'lastName': lastName,
      'username': username,
      'email': email,
      'password': password,
      'confirmPassword': confirmPassword,
      'countryIso2': countryIso2,
      'timeZoneId': timeZoneId,
    };
  }

  RegisterRequest copyWith({
    String? firstName,
    String? lastName,
    String? username,
    String? email,
    String? password,
    String? confirmPassword,
    String? countryIso2,
    String? timeZoneId,
  }) {
    return RegisterRequest(
      firstName: firstName ?? this.firstName,
      lastName: lastName ?? this.lastName,
      username: username ?? this.username,
      email: email ?? this.email,
      password: password ?? this.password,
      confirmPassword: confirmPassword ?? this.confirmPassword,
      countryIso2: countryIso2 ?? this.countryIso2,
      timeZoneId: timeZoneId ?? this.timeZoneId,
    );
  }
}
