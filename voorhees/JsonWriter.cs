﻿using System;
using System.Text;
using System.Collections.Generic;

namespace Voorhees {
    public static class JsonWriter {
        public static string ToJson(JsonValue json) {
            var result = new StringBuilder();
            WriteValue(result, json);
            return result.ToString();
        }

        /////////////////////////////////////////////////

        static void WriteValue(StringBuilder result, JsonValue value, int indentLevel = 0) {
            if (value == null) {
                result.Append("null");
                return;
            }

            switch (value.Type) {
                case JsonType.Array: WriteArray(result, value, indentLevel); break;
                case JsonType.Object: WriteObject(result, value, indentLevel); break;
                case JsonType.Float: result.Append((float) value); break;
                case JsonType.Int: result.Append((int) value); break;
                case JsonType.Boolean: result.Append((bool) value ? "true" : "false"); break;
                case JsonType.Null: result.Append("null"); break;
                case JsonType.String: WriteString(result, (string)value); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Replace special characters and wraps it in double quotes.
        /// </summary>
        /// <param name="result">Where to append the result.</param>
        /// <param name="value">The JSON string value.</param>
        internal static void WriteString(StringBuilder result, string value) {
            // Replace \ first because other characters expand into sequences that contain \
            result.Append('\"');
            result.Append(value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("/", "\\/")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t"));
            result.Append('\"');
        }

        static void WriteObject(StringBuilder result, JsonValue value, int indentLevel) {
            if (JsonConfig.CurrentConfig.PrettyPrint) {
                result.Append("{\n");

                bool first = true;
                foreach (var objectPair in value as IEnumerable<KeyValuePair<string, JsonValue>>) {
                    if (first) {
                        first = false;
                    } else {
                        result.Append(",\n");
                    }

                    for (int j = 0; j < indentLevel + 1; ++j) {
                        result.Append('\t');
                    }

                    result.Append('\"');
                    result.Append(objectPair.Key);
                    result.Append("\": ");
                    WriteValue(result, objectPair.Value, indentLevel + 1);
                }

                result.Append("\n");
                for (int j = 0; j < indentLevel; ++j) {
                    result.Append('\t');
                }

                result.Append('}');
            } else {
                result.Append('{');

                bool first = true;
                foreach (var objectPair in value as IEnumerable<KeyValuePair<string, JsonValue>>) {
                    if (first) {
                        first = false;
                    } else {
                        result.Append(",");
                    }

                    result.Append('\"');
                    result.Append(objectPair.Key);
                    result.Append("\":");
                    WriteValue(result, objectPair.Value);
                }

                result.Append('}');
            }
        }

        static void WriteArray(StringBuilder result, JsonValue value, int indentLevel) {
            if (JsonConfig.CurrentConfig.PrettyPrint) {
                result.Append("[\n");
                for (int i = 0; i < value.Count; ++i) {
                    for (int j = 0; j < indentLevel + 1; ++j) {
                        result.Append('\t');
                    }

                    WriteValue(result, value[i], indentLevel + 1);
                    if (i < value.Count - 1) {
                        result.Append(",\n");
                    }
                }

                result.Append('\n');
                for (int j = 0; j < indentLevel; ++j) {
                    result.Append('\t');
                }

                result.Append(']');
            } else {
                result.Append('[');
                for (int i = 0; i < value.Count; ++i) {
                    if (i != 0) {
                        result.Append(',');
                    }

                    WriteValue(result, value[i]);
                }

                result.Append(']');
            }
        }
    }
}
