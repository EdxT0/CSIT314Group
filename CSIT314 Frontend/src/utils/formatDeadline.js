export function formatDeadline(str) {
  if (!str) return "";

  // ← catch C# DateTime.MinValue
  if (str.startsWith("0001")) return "—";

  // backend sends dd-MM-yyyy — return as-is
  if (/^\d{2}-\d{2}-\d{4}$/.test(str)) {
    return str;
  }

  // fallback for ISO format
  const date = new Date(str);
  if (!isNaN(date) && date.getFullYear() > 1) {
    const dd = String(date.getDate()).padStart(2, "0");
    const mm = String(date.getMonth() + 1).padStart(2, "0");
    const yyyy = date.getFullYear();
    return `${dd}-${mm}-${yyyy}`;
  }

  return str;
}