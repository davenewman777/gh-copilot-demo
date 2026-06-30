import de from './de'
import en from './en'
import fr from './fr'

export const translations = {
  en,
  fr,
  de
} as const

export type LanguageCode = keyof typeof translations
export type TranslationKey = keyof typeof en

export const languageOptions: Array<{ code: LanguageCode; label: string }> = [
  { code: 'en', label: 'English' },
  { code: 'fr', label: 'Francais' },
  { code: 'de', label: 'Deutsch' }
]