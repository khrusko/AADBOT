namespace AADBOT_KarloHruskovec.Application.Common;

public readonly struct Option<T>
{
	private readonly T _value;
	public bool HasValue { get; }
	public T Value => HasValue ? _value : throw new InvalidOperationException();

	private Option(T value)
	{
		_value = value;
		HasValue = true;
	}

	public static Option<T> None => default;
	public static Option<T> Some(T value) => new(value);

	public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none) =>
		HasValue ? some(_value) : none();
}

public readonly struct Result<T>
{
	public bool IsSuccess { get; }
	public T Value { get; }
	public string Error { get; }

	private Result(T value)
	{
		IsSuccess = true;
		Value = value;
		Error = string.Empty;
	}

	private Result(string error)
	{
		IsSuccess = false;
		Value = default!;
		Error = error;
	}

	public static Result<T> Ok(T value) => new(value);
	public static Result<T> Fail(string error) => new(error);

	public TResult Match<TResult>(Func<T, TResult> ok, Func<string, TResult> fail) =>
		IsSuccess ? ok(Value) : fail(Error);
}
