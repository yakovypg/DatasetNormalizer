import re
import argparse

def configure_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description='Replace values in quotes')
    parser.add_argument('-i', '--input', type=str, required=True, help='Input file path')
    parser.add_argument('-o', '--output', type=str, required=True, help='Output file path')
    parser.add_argument('-t', '--to', type=str, required=True, help='String to replace the value with')

    return parser

def get_input_file_lines(path: str) -> list[str]:
    lines = None

    with open(path, 'r') as file:
        lines = file.readlines()

    return lines

def replace_values(lines: list[str], new_value: str, output_path: str) -> None:
    with open(output_path, 'w') as file:
        for line in lines:
            values_in_quotes = list(re.findall(r'"(.*?)"', line))

            if len(values_in_quotes) == 0:
                file.write(line)
                continue

            for value in values_in_quotes:
                value_in_quotes = '"' + value + '"'
                line = line.replace(value_in_quotes, new_value)

            file.write(line)

if __name__ == '__main__':
    parser = configure_parser()
    args = parser.parse_args()

    lines = get_input_file_lines(args.input)
    replace_values(lines, args.to, args.output)
