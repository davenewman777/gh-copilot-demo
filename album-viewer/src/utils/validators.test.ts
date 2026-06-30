import { describe, expect, it } from 'vitest'
import { validateDate, validateGuid, validateIPV6 } from './validators'

describe('validators', () => {
	it('validates French date strings', () => {
		const date = validateDate('29/02/2024')

		expect(date).toBeInstanceOf(Date)
		expect(date?.getFullYear()).toBe(2024)
		expect(date?.getMonth()).toBe(1)
		expect(date?.getDate()).toBe(29)
		expect(validateDate('31/02/2024')).toBeNull()
		expect(validateDate('2024-02-29')).toBeNull()
	})

	it('validates GUID strings', () => {
		expect(validateGuid('01234567-89ab-cdef-0123-456789abcdef')).toBe(true)
		expect(validateGuid('0123456789abcdef0123456789abcdef')).toBe(false)
		expect(validateGuid('not-a-guid')).toBe(false)
	})

	it('validates IPv6 address strings', () => {
		expect(validateIPV6('::1')).toBe(true)
		expect(validateIPV6('2001:db8::1')).toBe(true)
		expect(validateIPV6('192.168.0.1')).toBe(false)
		expect(validateIPV6('2001:::1')).toBe(false)
	})
})