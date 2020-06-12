using System;

namespace BurningKnight.util {
  public static class LoadScreenTips {
    public static string Generate() {
      return jokes[new Random().Next(jokes.Length)];
    }

    private static readonly string[] jokes = {
      "Maanex holds the secret to looping",
      "Redkey is the key",
      "Crosses on the walls mean something...",
      "Press R for a surprise!",
    };
  }
}