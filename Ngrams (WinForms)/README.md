Этот небольшой проект - реализация N-граммной языковой модели для продолжения слов на основе биграмм и триграмм. Делал я его где-то в середине весны 2021. Класс Parser парсит входной txt файл в список списков строк (список предложений в тексте) без разделителей. Класс Analyzer составляет на основе полученного списка частотный словарь биграмм и триграмм (частотная оценка вероятности). Класс Generator продолжает фразу на основе этого словаря и входного слова или нескольких слов. Юнит-тестов в этом проекте нет, хотя для первой его версии они были. Логика взаимодействия этих трёх классов между собой и внешними файлами, а также примитивный интерфейс, содержатся в MyForm.cs