export function matchesSearch(query: string, ...values: Array<string | number | null | undefined>): boolean {
  const normalizedQuery = query.trim().toLowerCase();

  if (!normalizedQuery) {
    return true;
  }

  return values
    .filter((value): value is string | number => value !== null && value !== undefined)
    .some((value) => value.toString().toLowerCase().includes(normalizedQuery));
}
