using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides functions to log debug.log messages with the callers type name as prefix. Useful for analysing log files outside of Unity 
/// </summary>
public static class TypeLogger
{
    static List<string> _blacklist = new List<string>();

    /// <summary>
    /// Generic log function which adds caller type prefix to log messages, i.e. "CallerName: <message>"
    /// </summary>
    /// <typeparam name="T">caller type</typeparam>
    /// <param name="caller">caller instance</param>
    /// <param name="message">String message to log</param>
    /// <param name="logLevel">Log level. 1 == Log; 2 == LogWarning; 3 == LogError</param>
    /// <remarks>Useful for debugging builds with android logcat.</remarks>
    public static void TypeLog<T>(T caller, object message, int logLevel)
    {
        // Prevent log if caller is blacklisted
        foreach (var blacklisted in _blacklist)
        {
            if (caller.GetType().ToString().Contains(blacklisted))
            {
                return;
            }
        }

        if (logLevel == 1)
        {
            Debug.Log(TypeMessagePrefix(caller) + message);
        }
        else if (logLevel == 2)
        {
            Debug.LogWarning(TypeMessagePrefix(caller) + message);
        }
        else if (logLevel == 3)
        {
            Debug.LogError(TypeMessagePrefix(caller) + message);
        }
        else
        {
            Debug.Log(TypeMessagePrefix(caller) + message);
        }
    }

    public static void AddToBlacklist(string[] blacklist)
    {
        // Add callers to blacklist
        foreach (var caller in blacklist)
        {
            if (!_blacklist.Contains(caller))
            {
                _blacklist.Add(caller);
            }
        }
    }

    public static string TypeMessagePrefix<T>(T caller)
    {
        return new string("[" + caller.GetType().ToString() + "] ");
    }

    public static void LogException(Exception exception)
    {
        if (exception == null) return;

        string errorMessage = $"Exception: {exception.Message}\nStack Trace: {exception.StackTrace}";

        if (exception.InnerException != null)
        {
            errorMessage += $"\nInner Exception: {exception.InnerException.Message}\n{exception.InnerException.StackTrace}";
        }

        if (exception is AggregateException aggregateException)
        {
            foreach (var innerException in aggregateException.InnerExceptions)
            {
                errorMessage += $"\nAggregate Inner Exception: {innerException.Message}\n{innerException.StackTrace}";
            }
        }

        Debug.LogError(errorMessage);
    }
}

