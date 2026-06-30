export const validateDate = (value: string): Date | null => {
	const match = value.trim().match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/)

	if (!match) {
		return null
	}

	const day = Number(match[1])
	const month = Number(match[2])
	const year = Number(match[3])

	const date = new Date(year, month - 1, day)

	if (
		date.getFullYear() !== year ||
		date.getMonth() !== month - 1 ||
		date.getDate() !== day
	) {
		return null
	}

	return date
}

export const validateGuid = (value: string): boolean => {
	return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(value.trim())
}

export const validateIPV6 = (value: string): boolean => {
	const address = value.trim()

	if (!address) {
		return false
	}

	try {
		new URL(`http://[${address}]/`)
		return true
	} catch {
		return false
	}
}
